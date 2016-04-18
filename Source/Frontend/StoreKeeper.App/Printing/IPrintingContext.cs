using System.Collections.Generic;

namespace StoreKeeper.App.Printing
{
    public interface IPrintingContext
    {
        string Label { get; }

        IEnumerable<PrintColumnDefinition> Columns { get; }

        List<object> DataSource { get; }
    }
}