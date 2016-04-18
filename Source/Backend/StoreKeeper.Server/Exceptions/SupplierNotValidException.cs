using System;

namespace StoreKeeper.Server.Exceptions
{
    public class SupplierNotValidException : InternalServerException
    {
        public SupplierNotValidException(Type type)
            : base(type, LogId.SupplierParsingFault, "Supplier cannot be parsed.")
        {
        }
    }
}