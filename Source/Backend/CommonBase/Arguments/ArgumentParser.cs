using System;
using System.Collections.Generic;

namespace CommonBase.Arguments
{
    public class ArgumentParser
    {
        private Dictionary<ArgId, Argument> _arguments;

        public ArgumentParser()
        {
            _arguments = new Dictionary<ArgId, Argument>();
        }

        public void Parse(string[] arguments)
        {
        }

        public void RegisterArgument(Argument argument)
        {
            ArgId argId = CreateArgId(argument.GetType());
            _arguments.Add(argId, argument);
        }

        #region Internals and Helpers

        private ArgId CreateArgId(Type type)
        {
            return new ArgId(type);
        }

        #endregion
    }
}