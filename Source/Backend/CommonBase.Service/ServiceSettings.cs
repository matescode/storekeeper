namespace CommonBase.Service
{
    internal class ServiceSettings
    {
        private const string ModeArg = "--mode";

        public ServiceSettings(string[] args)
        {
            Mode = ServiceMode.Service;
            ParseArguments(args);
        }

        #region Properties

        public ServiceMode Mode { get; private set; }

        #endregion

        #region Internals and Helpers

        private void ParseArguments(string[] args)
        {
            foreach (string argument in args)
            {
                if (!argument.StartsWith(ModeArg))
                {
                    continue;
                }

                int splitIndex = argument.IndexOf("=");
                if (splitIndex >= 0)
                {
                    string mode = argument.Substring(splitIndex + 1);
                    if (mode.ToLower() == "service")
                    {
                        Mode = ServiceMode.Service;
                    }
                    else if (mode.ToLower() == "console")
                    {
                        Mode = ServiceMode.Console;
                    }
                }
            }
        }

        #endregion
    }
}