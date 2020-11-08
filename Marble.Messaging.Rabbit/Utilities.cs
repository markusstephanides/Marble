namespace Marble.Messaging.Rabbit
{
    public static class Utilities
    {
        public const string AmqDirectExchange = "";
        public const string AmqFanoutExchange = "amq.fanout";
        public const string AmqDirectReplyToQueue = "amq.rabbitmq.reply-to";
        public const string ProvisionalProtocolVersion = "development";
        public const string ProvisionalEnvironment = "development";
        public const int DefaultTimeout = 5000;
    }
}