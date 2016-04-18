using System;
using CommonBase.Application;

namespace StoreKeeper.Service
{
    internal class ServiceApplicationInfo : ApplicationInfo
    {
        protected override string NameString
        {
            get { return "StoreKeeper.Service"; }
        }

        protected override string LongNameString
        {
            get { return "StoreKeeper Service"; }
        }

        protected override string DescriptionString
        {
            get { return "StoreKeeper Service Module - works as access point between POHODA and StoreKeeper client applications."; }
        }

        protected override string WebUrl
        {
            get { return "http://www.flajzar.net"; }
        }

        protected override string CompanyName
        {
            get { return "Flajzar, s.r.o."; }
        }

        protected override Version ApplicationVersion
        {
            get
            {
                return new Version(1, 0);
            }
        }
    }
}