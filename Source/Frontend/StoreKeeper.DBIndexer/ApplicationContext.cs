using CommonBase.Application;
using CommonBase.Log;

namespace StoreKeeper.DBIndexer
{
    public class ApplicationContext : CommonBase.Application.ApplicationContext
    {
        public ApplicationContext(ILog log)
            : base(ApplicationType.Console, log)
        {
        }

        #region Overrides

        protected override ApplicationConfiguration CreateConfiguration()
        {
            return new Server.Config();
        }

        protected override IApplicationInfo CreateApplicationInfo()
        {
            return null;
        }

        public static new Server.Config Config
        {
            get { return CommonBase.Application.ApplicationContext.Config as Server.Config; }
        }

        #endregion
    }
}