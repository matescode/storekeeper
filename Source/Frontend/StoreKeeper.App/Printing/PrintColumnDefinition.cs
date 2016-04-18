namespace StoreKeeper.App.Printing
{
    public class PrintColumnDefinition
    {
        public PrintColumnDefinition(string header, string bindingProperty, int width = 100, bool rightAlign = false)
        {
            Header = header;
            BindingProperty = bindingProperty;
            Width = width;
            RightAlign = rightAlign;
        }

        public string Header { get; private set; }

        public string BindingProperty { get; private set; }

        public int Width { get; private set; }

        public bool RightAlign { get; private set; }
    }
}