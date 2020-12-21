using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using akarnokd.reactive_extensions;
using Marble.Messaging.Abstractions;
using Marble.Messaging.Contracts.Abstractions;
using Marble.Messaging.Contracts.Models;
using Marble.Messaging.Contracts.Models.Stream;
using Marble.Messaging.Extensions;
using Marble.Messaging.Transformers;
using Marble.Messaging.Utilities;
using Microsoft.Extensions.Logging;

namespace Marble.Messaging.Services
{
    public class DefaultMessagingClient : IMessagingClient
    {
        private readonly IControllerRegistry controllerRegistry;
        private readonly ILogger<DefaultMessagingClient> logger;
        private readonly IObservable<RemoteMessage> messageFeed;
        private readonly IMessagingAdapter messagingAdapter;
        private readonly ISerializationAdapter serializationAdapter;
        private readonly IStreamManager streamManager;

        public DefaultMessagingClient(IMessagingAdapter messagingAdapter, ISerializationAdapter serializationAdapter,
            IControllerRegistry controllerRegistry, IStreamManager streamManager,
            ILogger<DefaultMessagingClient> logger)
        {
            this.messagingAdapter = messagingAdapter;
            this.serializationAdapter = serializationAdapter;
            this.controllerRegistry = controllerRegistry;
            this.streamManager = streamManager;
            this.logger = logger;
            this.messageFeed = this.messagingAdapter.MessageFeed;
        }

        public IObservable<TResult> InvokeProcedureStream<TResult>(RequestMessage requestMessage)
        {
            requestMessage.Correlation ??= Guid.NewGuid().ToString();

            var procedureResultStream = this.ConstructResponseStream<TResult>(requestMessage);
            // TODO: Lets be 100% sure that this doesn't cause any memory-leaks
            procedureResultStream.Subscribe(_ => { }, _ => { });
            this.messagingAdapter.SendRemoteMessage(requestMessage.ToRemoteMessage(this.serializationAdapter));

            return procedureResultStream;
        }

        public Task<TResult> InvokeProcedureAsync<TResult>(RequestMessage requestMessage)
        {
            var stopwatch = Stopwatch.StartNew();
            return this.InvokeProcedureStream<TResult>(requestMessage)
                .FirstAsync()
                .Timeout(TimeSpan.FromSeconds(Constants.DefaultTimeoutSeconds))
                .Do(result =>
                {
                    this.logger.LogInformation(
                        "Received response of type {responseType} for procedure invocation {procedurePath} after {elapsedMilliseconds} ms",
                        result.GetType().Name, ProcedurePath.FromRequestMessage(requestMessage),
                        stopwatch.ElapsedMilliseconds);
                }, error =>
                {
                    this.logger.LogError(error,
                        "Error for procedure invocation {procedurePath} after {elapsedMilliseconds} ms",
                        ProcedurePath.FromRequestMessage(requestMessage), stopwatch.ElapsedMilliseconds);
                })
                .ToTask();
        }

        public Task CallProcedureAsync(RequestMessage requestMessage)
        {
            return this.messagingAdapter.SendRemoteMessage(requestMessage.ToRemoteMessage(this.serializationAdapter));
        }

        public IObservable<TResult> InvokeProcedureStream<TResult>(string controller, string procedure,
            params object[] parameters)
        {
            return this.InvokeProcedureStream<TResult>(new RequestMessage(controller, procedure, parameters));
        }

        public Task<TResult> InvokeProcedureAsync<TResult>(string controller, string procedure,
            params object[] parameters)
        {
            return this.InvokeProcedureAsync<TResult>(new RequestMessage(controller, procedure, parameters));
        }

        public Task CallProcedureAsync(string controller, string procedure, params object[] parameters)
        {
            return this.CallProcedureAsync(new RequestMessage(controller, procedure, parameters));
        }

        private IObservable<TResult> ConstructResponseStream<TResult>(RequestMessage requestMessage)
        {
            return this.messageFeed
                .Cache()
                .Where(message =>
                    message.MessageType == MessageType.ResponseMessage)
                .Select(message => message.ToResponseMessage(this.serializationAdapter))
                .Where(responseMessage => responseMessage.Correlation == requestMessage.Correlation)
                .Select(responseMessage => this.streamManager
                    .TypedStreamToObservable(
                        NetworkStream.ToTypedStream<TResult>(responseMessage.Stream, this.serializationAdapter))
                    .Materialize())
                .Switch()
                .Dematerialize();
        }
    }
}