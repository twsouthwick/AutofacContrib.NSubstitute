using System;
using System.Collections.Generic;

namespace AutofacContrib.NSubstitute
{
    public class DefaultAutoSubstituteBuilder
    {
        private readonly List<Action<AutoSubstituteBuilder>> _actions;

        public DefaultAutoSubstituteBuilder()
        {
            _actions = new List<Action<AutoSubstituteBuilder>>();
        }

        public bool IsLocked { get; private set; }

        public DefaultAutoSubstituteBuilder ConfigureDefault(Action<AutoSubstituteBuilder> configure)
        {
            if (IsLocked)
            {
                throw new InvalidOperationException("Cannot add default actions to a locked default builder");
            }

            _actions.Add(configure);

            return this;
        }

        public void Lock()
        {
            IsLocked = true;
        }

        internal void ConfigureBuilder(AutoSubstituteBuilder builder)
        {
            foreach (var action in _actions)
            {
                action(builder);
            }
        }
    }
}