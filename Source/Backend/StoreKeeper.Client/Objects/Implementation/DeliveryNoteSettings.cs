using System;
using StoreKeeper.Client.Objects.DataProxy;

namespace StoreKeeper.Client.Objects.Implementation
{
    internal class DeliveryNoteSettings : BaseObject<DeliveryNoteSettingsDataProxy>, IDeliveryNoteSettings
    {
        public DeliveryNoteSettings(DeliveryNoteSettingsDataProxy dataProxy)
            : base(dataProxy)
        {
        }

        #region IDeliveryNoteSettings Implementation

        public string Parlor
        {
            get { return Proxy.Parlor; }
            set { Proxy.Parlor = value; }
        }

        public string Street
        {
            get { return Proxy.Street; }
            set { Proxy.Street = value; }
        }

        public string Number
        {
            get { return Proxy.Number; }
            set { Proxy.Number = value; }
        }

        public string ZipCode
        {
            get { return Proxy.ZipCode; }
            set { Proxy.ZipCode = value; }
        }

        public string City
        {
            get { return Proxy.City; }
            set { Proxy.City = value; }
        }

        public string Phone
        {
            get { return Proxy.Phone; }
            set { Proxy.Phone = value; }
        }

        public string CellPhone
        {
            get { return Proxy.CellPhone; }
            set { Proxy.CellPhone = value; }
        }

        public string Email
        {
            get { return Proxy.Email; }
            set { Proxy.Email = value; }
        }

        public string Web
        {
            get { return Proxy.Web; }
            set { Proxy.Web = value; }
        }

        public void Save()
        {
            Proxy.Save();
        }

        #endregion
    }
}