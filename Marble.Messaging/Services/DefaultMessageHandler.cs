using System;
using System.Diagnostics;
using System.Reactive.Linq;
using Marble.Messaging.Abstractions;
using Marble.Messaging.Contracts.Abstractions;
using Marble.Messaging.Contracts.Models;
using Marble.Messaging.Contracts.Models.Stream;
using Marble.Messaging.Exceptions;
using Marble.Messaging.Extensions;
using Microsoft.Extensions.Logging;

namespace Marble.Messaging.Services
{
    public class DefaultMessageHandler : IMessageHandler
    {
        private readonly IControllerRegistry controllerRegistry;
        private readonly ILogger<DefaultMessageHandler> logger;
        private readonly IMessagingAdapter messagingAdapter;
        private readonly ISerializationAdapter serializationAdapter;
        private readonly IStreamManager streamManager;

        public DefaultMessageHandler(IMessagingAdapter messagingAdapter, IControllerRegistry controllerRegistry,
            IStreamManager streamManager, ISerializationAdapter serializationAdapter,
            ILogger<DefaultMessageHandler> logger)
        {
            this.messagingAdapter = messagingAdapter;
            this.controllerRegistry = controllerRegistry;
            this.streamManager = streamManager;
            this.serializationAdapter = serializationAdapter;
            this.logger = logger;
        }

        public void Initialize()
        {
            this.InitializeMessageReceivingChain();
        }

        private void InitializeMessageReceivingChain()
        {
            this.messagingAdapter.MessageFeed
                .Where(remoteMessage => remoteMessage.MessageType == MessageType.RequestMessage)
                .Select(remoteMessage => remoteMessage.ToRequestMessageContext(this.serializationAdapter))
                .Subscribe(this.HandleRequestMessage);
        }

        private void HandleRequestMessage(RequestMessageContext requestMessageContext)
        {
            // TODO check which thread this is
            var stopwatch = Stopwatch.StartNew();
            var requestMessage = requestMessageContext.RequestMessage;
            try
            {
                var messageHandlingResult = this.controllerRegistry.InvokeProcedure(requestMessage.Controller,
                    requestMessage.Procedure, requestMessage.Arguments);

                switch (messageHandlingResult.Type)
                {
                    case MessageHandlingResultType.Single:
                        this.messagingAdapter.SendRemoteMessage(
                            new ResponseMessage
                            {
                                Stream = BasicStream.FromResult(messageHandlingResult.Result),
                                Correlation = requestMessage.Correlation
                            }.ToRemoteMessage(requestMessageContext, this.serializationAdapter));
                        this.logger.LogInformation(
                            "Handled request to {controller}:{procedure} successfully in {elapsedMilliseconds} ms with result of {resultType}.",
                            requestMessage.Controller, requestMessage.Procedure, stopwatch.ElapsedMilliseconds,
                            messageHandlingResult.Result.GetType().Name);
                        break;
                    case MessageHandlingResultType.Stream:
                        messageHandlingResult.ResultStream!.Subscribe(item =>
                        {
                            this.messagingAdapter.SendRemoteMessage(
                                new ResponseMessage
                                {
                                    Stream = BasicStream.FromNotification(item),
                                    Correlation = requestMessage.Correlation
                                }.ToRemoteMessage(requestMessageContext, this.serializationAdapter));
                        }, error =>
                        {
                            this.messagingAdapter.SendRemoteMessage(
                                new ResponseMessage
                                {
                                    Stream = BasicStream.FromError(messageHandlingResult.Result),
                                    Correlation = requestMessage.Correlation
                                }.ToRemoteMessage(requestMessageContext, this.serializationAdapter));
                        }, () =>
                        {
                            this.messagingAdapter.SendRemoteMessage(
                                new ResponseMessage
                                {
                                    Stream = BasicStream.Completed,
                                    Correlation = requestMessage.Correlation
                                }.ToRemoteMessage(requestMessageContext, this.serializationAdapter));
                        });

                        this.logger.LogInformation(
                            "Opened stream after request to {controller}:{procedure} successfully in {elapsedMilliseconds} ms.",
                            requestMessage.Controller, requestMessage.Procedure, stopwatch.ElapsedMilliseconds);
                        break;
                    case MessageHandlingResultType.Void:
                        this.logger.LogInformation(
                            "Handled request to {controller}:{procedure} successfully in {elapsedMilliseconds} ms with no result.",
                            requestMessage.Controller, requestMessage.Procedure, stopwatch.ElapsedMilliseconds);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception e)
            {
                switch (e)
                {
                    case ProcedureInvocationException _:
                        this.logger.LogWarning(e.InnerException,
                            "Procedure invocation to {controller}:{procedure} failed due to logic exception.",
                            requestMessage.Controller, requestMessage.Procedure);
                        e = e.InnerException!;
                        break;
                    case ProcedureResultConversionException _:
                        this.logger.LogError(e.InnerException,
                            "Procedure invocation to {controller}:{procedure} failed because result conversion resulted in exception.",
                            requestMessage.Controller, requestMessage.Procedure);
                        e = e.InnerException is AggregateException aggregateException &&
                            aggregateException.InnerExceptions.Count == 1
                            ? aggregateException.InnerException!
                            : e.InnerException!;
                        break;
                    default:
                        this.logger.LogError(e,
                            "Failed to handle request to {controller}:{procedure}.", requestMessage.Controller,
                            requestMessage.Procedure);
                        break;
                }

                var responseMessage = new ResponseMessage
                {
                    Correlation = requestMessage.Correlation,
                    Stream = BasicStream.FromError(e)
                };

                this.messagingAdapter.SendRemoteMessage(
                    responseMessage.ToRemoteMessage(requestMessageContext, this.serializationAdapter));
            }
        }
    }
}