using CommonBase.Application;
using CommonBase.Log;

namespace StoreKeeper.Service.Contexts
{
    public class ApplicationContext : CommonBase.Application.ApplicationContext
    {
        public ApplicationContext(ILog log)
            : base(ApplicationType.Service, log)
        {
        }

        #region Overrides

        protected override ApplicationConfiguration CreateConfiguration()
        {
            return new Server.Config();
        }

        protected override IApplicationInfo CreateApplicationInfo()
        {
            return new ServiceApplicationInfo();
        }

        public static new Server.Config Config
        {
            get { return CommonBase.Application.ApplicationContext.Config as Server.Config; }
        }

        #endregion
    }
}