using System;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Channels;
using System.Threading.Tasks;
using Marble.Messaging.Abstractions;
using Marble.Messaging.Contracts.Abstractions;
using Marble.Messaging.Contracts.Models;
using Marble.Messaging.Extensions;
using Marble.Messaging.Utilities;
using Newtonsoft.Json;

namespace Marble.Messaging.Services
{
    public class DefaultMessagingClient : IMessagingClient
    {
        private readonly IMessagingAdapter messagingAdapter;
        private readonly ISerializationAdapter serializationAdapter;
        private readonly IControllerRegistry controllerRegistry;
        private readonly IObservable<RemoteMessage> messageFeed;
        private readonly IStreamManager streamManager;

        public DefaultMessagingClient(IMessagingAdapter messagingAdapter, ISerializationAdapter serializationAdapter, IControllerRegistry controllerRegistry, IStreamManager streamManager)
        {
            this.messagingAdapter = messagingAdapter;
            this.serializationAdapter = serializationAdapter;
            this.controllerRegistry = controllerRegistry;
            this.streamManager = streamManager;
            this.messageFeed = this.messagingAdapter.MessageFeed;
        }

        public IObservable<TResult> InvokeProcedureStream<TResult>(RequestMessage requestMessage)
        {
            requestMessage.Correlation ??= Guid.NewGuid().ToString();
            
            var procedureResultStream = this.ConstructResponseStream<TResult>(requestMessage);
            this.messagingAdapter.SendRemoteMessage(requestMessage.ToRemoteMessage(this.serializationAdapter));
            
            return procedureResultStream;
        }

        public Task<TResult> InvokeProcedureAsync<TResult>(RequestMessage requestMessage)
        {
            return this.InvokeProcedureStream<TResult>(requestMessage)
                .FirstAsync()
                .Timeout(TimeSpan.FromSeconds(Constants.DefaultTimeoutSeconds))
                .ToTask();
        }

        public Task CallProcedureAsync(RequestMessage requestMessage)
        {
            return this.messagingAdapter.SendRemoteMessage(requestMessage.ToRemoteMessage(this.serializationAdapter));
        }

        public IObservable<TResult> InvokeProcedureStream<TResult>(string controller, string procedure, params object[] parameters)
        {
            return this.InvokeProcedureStream<TResult>(new RequestMessage(controller, procedure, parameters));
        }

        public Task<TResult> InvokeProcedureAsync<TResult>(string controller, string procedure, params object[] parameters)
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
                .Where(message =>
                    message.MessageType == MessageType.ResponseMessage)
                .Select(message => message.ToResponseMessage(this.serializationAdapter))
                .Where(responseMessage => responseMessage.Correlation == requestMessage.Correlation)
                .Select(responseMessage => this.streamManager.StreamToObservable<TResult>(responseMessage.Stream))
                .Switch();
        } 
    }
}