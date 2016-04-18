using StoreKeeper.Common;

namespace StoreKeeper.Client.Objects.DataProxy
{
    internal class DeliveryNoteSettingsDataProxy : ProxyBase
    {
        public DeliveryNoteSettingsDataProxy(IDataChange dataChange)
            : base(dataChange)
        {
        }

        #region Properties

        public string Parlor { get; set; }

        public string Street { get; set; }

        public string Number { get; set; }

        public string ZipCode { get; set; }

        public string City { get; set; }

        public string Phone { get; set; }

        public string CellPhone { get; set; }

        public string Email { get; set; }

        public string Web { get; set; }

        #endregion

        #region ProxyBase Implementation

        internal override void Load()
        {
            using (StoreKeeperDataContext dataContext = new StoreKeeperDataContext())
            {
                Parlor = dataContext.GetStringConstant("DN_Parlor");
                Street = dataContext.GetStringConstant("DN_Street");
                Number = dataContext.GetStringConstant("DN_Number");
                ZipCode = dataContext.GetStringConstant("DN_Zip");
                City = dataContext.GetStringConstant("DN_City");
                Phone = dataContext.GetStringConstant("DN_Phone");
                CellPhone = dataContext.GetStringConstant("DN_CellPhone");
                Email = dataContext.GetStringConstant("DN_Email");
                Web = dataContext.GetStringConstant("DN_Web");
            }
        }

        #endregion

        #region Internals and Helpers

        internal void Save()
        {
            using (StoreKeeperDataContext dataContext = new StoreKeeperDataContext())
            {
                dataContext.SetStringConstant("DN_Parlor", Parlor);
                dataContext.SetStringConstant("DN_Street", Street);
                dataContext.SetStringConstant("DN_Number", Number);
                dataContext.SetStringConstant("DN_Zip", ZipCode);
                dataContext.SetStringConstant("DN_City", City);
                dataContext.SetStringConstant("DN_Phone", Phone);
                dataContext.SetStringConstant("DN_CellPhone", CellPhone);
                dataContext.SetStringConstant("DN_Email", Email);
                dataContext.SetStringConstant("DN_Web", Web);
                
                dataContext.SaveChanges();
            }
        }

        #endregion
    }
}