﻿using System;

namespace Marble.Core.Declaration
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MarbleControllerAttribute : Attribute
    {
        public string? Name { get; set; }
    }
}