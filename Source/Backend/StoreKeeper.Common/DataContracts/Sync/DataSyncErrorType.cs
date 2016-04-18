using System.Runtime.Serialization;

namespace StoreKeeper.Common.DataContracts.Sync
{
    [DataContract]
    public enum DataSyncErrorType
    {
        [EnumMember]
        Unknown,
        [EnumMember]
        Runtime,
        [EnumMember]
        MissingArticle,
        [EnumMember]
        MissingProductArticle
    }
}