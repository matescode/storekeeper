using System;

namespace CommonBase.Application
{
    public class ApplicationInfo : IApplicationInfo
    {
        #region IApplicationInfo Methods

        public string Name
        {
            get { return NameString; }
        }

        public string LongName
        {
            get { return LongNameString; }
        }

        public string Description
        {
            get { return DescriptionString; }
        }

        public Version Version
        {
            get { return ApplicationVersion; }
        }

        public string Company
        {
            get { return CompanyName; }
        }

        public string Web
        {
            get { return WebUrl; }
        }

        #endregion

        #region Virtual Getters of IApplicationInfo Properties

        protected virtual string NameString
        {
            get
            {
                return "CommonBase Framework";
            }
        }

        protected virtual string LongNameString
        {
            get
            {
                return "CommonBase Framework";
            }
        }

        protected virtual string DescriptionString
        {
            get
            {
                return string.Empty;
            }
        }

        protected virtual Version ApplicationVersion
        {
            get
            {
                return new Version(1, 0, 0, 0);
            }
        }

        protected virtual string CompanyName
        {
            get
            {
                return "MateSCode Production";
            }
        }

        protected virtual string WebUrl
        {
            get
            {
                return "http://www.matescode.net";
            }
        }

        #endregion
    }
}