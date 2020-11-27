﻿using System;
using System.Collections.Generic;
using Marble.Messaging.Abstractions;

namespace Marble.Messaging.Contracts.Configuration
{
    public class MessagingConfiguration
    {
        public string ConnectionString { get; set; }
        public int DefaultTimeoutInSeconds { get; set; }
        public Type SerializationAdapterType { get; set; }
        public List<string> KnownProcedurePaths { get; set; }
        public List<IConverter> TypeConverters { get; set; }
    }
}