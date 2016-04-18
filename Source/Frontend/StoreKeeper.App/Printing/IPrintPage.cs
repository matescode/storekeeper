using System.Windows;

namespace StoreKeeper.App.Printing
{
    public interface IPrintPage
    {
        UIElement PrintElement { get; } 
    }
}