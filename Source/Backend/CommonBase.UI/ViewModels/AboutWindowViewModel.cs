using System;
using System.Windows.Media;
using CommonBase.Application;
using CommonBase.UI.Localization;

namespace CommonBase.UI.ViewModels
{
    internal class AboutWindowViewModel
    {
        private readonly IApplicationInfo _applicationInfo;
        private readonly ImageSource _applicationIcon;
        private readonly string _eula;
        private readonly string _copyright;

        public AboutWindowViewModel(IApplicationInfo applicationInfo, ImageSource applicationIcon, string eula, string copyright)
        {
            _applicationInfo = applicationInfo;
            _applicationIcon = applicationIcon;
            _eula = eula;
            _copyright = copyright;
        }

        #region Properties

        public string WindowTitle
        {
            get { return String.Format("{0} {1}", "About".Localize(), _applicationInfo.Name); }
        }

        public ImageSource ApplicationIcon
        {
            get { return _applicationIcon; }
        }

        public string ApplicationName
        {
            get { return _applicationInfo.Name; }
        }

        public string Version
        {
            get { return _applicationInfo.Version.ToString(2); }
        }

        public string AuthorizedUser
        {
            get { return _applicationInfo.Company; }
        }

        public string AuthorizedUserWeb
        {
            get { return _applicationInfo.Web; }
        }

        public string Eula
        {
            get { return _eula.Localize(); }
        }

        public string Copyright
        {
            get { return _copyright.Localize(); }
        }

        #endregion
    }
}