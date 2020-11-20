using System;


namespace Marble.Messaging.Contracts.Playground
{
    public class RemoteProcedureCall
    {
        readonly string COMPLETE = "COMPLETE";
        readonly string ERROR = "COMPLETE";

        private Func<int> RpcFnThatShouldBeCalled;

        void onInitialCall()
        {

        }

        void onDataReceived(object data_)
        {
            var data = new[] { new[]{"100", "complete"} };
        }
    }
}
