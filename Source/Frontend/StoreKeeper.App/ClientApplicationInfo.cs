using System;
using CommonBase.Application;

namespace StoreKeeper.App
{
    internal class ClientApplicationInfo : ApplicationInfo
    {
        protected override string NameString
        {
            get { return "StoreKeeper"; }
        }

        protected override string LongNameString
        {
            get { return "StoreKeeper Application"; }
        }

        protected override string DescriptionString
        {
            get { return "StoreKeeper Application Module - client application"; }
        }

        protected override string WebUrl
        {
            get { return "http://www.flajzar.cz"; }
        }

        protected override string CompanyName
        {
            get { return "FLAJZAR, s.r.o."; }
        }

        protected override Version ApplicationVersion
        {
            get
            {
                return new Version(2, 0);
            }
        }
    }
}