﻿using System;
using System.Diagnostics;
using System.Reactive.Linq;
using Marble.Messaging.Abstractions;
using Marble.Messaging.Contracts.Abstractions;
using Marble.Messaging.Contracts.Models;
using Marble.Messaging.Contracts.Models.Stream;
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
                var result = this.controllerRegistry.InvokeProcedure(requestMessage.Controller,
                    requestMessage.Procedure, requestMessage.Arguments);

                if (result != null)
                {
                    var responseMessage = new ResponseMessage
                    {
                        Correlation = requestMessage.Correlation,
                        Stream = BasicStream.FromResult(result)
                    };

                    this.messagingAdapter.SendRemoteMessage(
                        responseMessage.ToRemoteMessage(requestMessageContext, this.serializationAdapter));

                    this.logger.LogInformation(
                        $"Handled request to {requestMessage.Controller}:{requestMessage.Procedure} successfully in {stopwatch.ElapsedMilliseconds} ms with result of {result.GetType().Name}.");
                }
                else
                {
                    this.logger.LogInformation(
                        $"Handled request to {requestMessage.Controller}:{requestMessage.Procedure} successfully in {stopwatch.ElapsedMilliseconds} ms with no result.");
                }
            }
            catch (Exception e)
            {
                this.logger.LogError(e,
                    $"Failed to handle request to {requestMessage.Controller}:{requestMessage.Procedure} within {stopwatch.ElapsedMilliseconds} ms.");
                var responseMessage = new ResponseMessage
                {
                    Correlation = requestMessage.Correlation,
                    Stream = BasicStream.FromError(e)
                };

                this.messagingAdapter.SendRemoteMessage(
                    responseMessage.ToRemoteMessage(requestMessageContext, this.serializationAdapter));
                throw;
            }
        }
    }
}