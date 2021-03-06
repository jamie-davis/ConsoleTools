﻿using System;

namespace ConsoleToolkit.Exceptions
{
    internal class CommandHandlerMustHaveDefaultConstructor : Exception
    {
        public Type Type { get; private set; }

        public CommandHandlerMustHaveDefaultConstructor(Type type)
            : base(string.Format("No default constructor found in {0}", type))
        {
            Type = type;
        }
    }
}