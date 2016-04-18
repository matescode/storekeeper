namespace StoreKeeper.Client.Objects
{
    public interface IDeliveryNoteSettings : IClientObject
    {
        string Parlor { get; set; }

        string Street { get; set; }

        string Number { get; set; }

        string ZipCode { get; set; }

        string City { get; set; }

        string Phone { get; set; }

        string CellPhone { get; set; }

        string Email { get; set; }

        string Web { get; set; }

        void Save();
    }
}