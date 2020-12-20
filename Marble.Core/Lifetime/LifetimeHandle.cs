using System;
using System.Collections.Generic;

namespace Marble.Core.Lifetime
{
    public class LifetimeHandle<TArgument>
    {
        private readonly List<Action<TArgument>> registeredActions;
        private bool hasBeenInvoked;
        private TArgument invocationArgument;

        public LifetimeHandle()
        {
            this.registeredActions = new List<Action<TArgument>>();
        }

        public void Register(Action<TArgument> action)
        {
            if (this.hasBeenInvoked)
            {
                action(this.invocationArgument);
            }
            else
            {
                this.registeredActions.Add(action);
            }
        }

        internal void Invoke(TArgument argument)
        {
            this.invocationArgument = argument;
            this.hasBeenInvoked = true;
            this.registeredActions.ForEach(action => action(this.invocationArgument));
        }
    }
}