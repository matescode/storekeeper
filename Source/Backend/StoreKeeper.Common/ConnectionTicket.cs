using System.Runtime.Serialization;

namespace StoreKeeper.Common
{
    [DataContract]
    public class ConnectionTicket
    {
        [DataMember]
        public string Username { get; set; }

        [DataMember]
        public string SecurityToken { get; set; }

        [DataMember]
        public string ClientComputer { get; set; }

        [DataMember]
        public int Port { get; set; }
    }
}