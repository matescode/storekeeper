using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace StoreKeeper.App.Printing.DeliveryNote
{
    public class DeliveryNoteFirstPage : DeliveryNotePageBase
    {
        public DeliveryNoteFirstPage(DeliveryNotePrintContext printContext, int pageCount)
            : base(printContext, 1, pageCount)
        {
        }

        #region Overrides

        protected override void CreateGridContent(Grid grid)
        {
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            if (PageCount == 1)
            {
                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(100) });
                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(100) });
            }

            Grid supplierInfoGrid = new Grid
                {
                    ShowGridLines = false,
                };

            supplierInfoGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(20) });
            supplierInfoGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(20) });
            supplierInfoGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(20) });
            supplierInfoGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(20) });
            supplierInfoGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(20) });
            supplierInfoGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(20) });
            supplierInfoGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(20) });
            supplierInfoGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(20) });
            supplierInfoGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(20) });
            supplierInfoGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(20) });
            supplierInfoGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(20) });
            supplierInfoGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(20) });
            supplierInfoGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(20) });

            supplierInfoGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(177) });
            supplierInfoGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(177) });

            SetTextToGrid(supplierInfoGrid, "Dodavatel", 0, 0);
            SetTextToGrid(supplierInfoGrid, PrintContext.DataSource.Supplier.Company, 1, 0, true);
            SetTextToGrid(supplierInfoGrid, String.Format("{0} {1}", PrintContext.DataSource.Supplier.Street, PrintContext.DataSource.Supplier.Number), 2, 0, true);
            SetTextToGrid(supplierInfoGrid, String.Format("{0} {1}", PrintContext.DataSource.Supplier.ZipCode, PrintContext.DataSource.Supplier.City), 3, 0, true);
            SetTextToGrid(supplierInfoGrid, "Provozovna", 3, 1);
            SetTextToGrid(supplierInfoGrid, String.Format("IČ: {0}", PrintContext.DataSource.Supplier.CompanyId), 4, 0);
            SetTextToGrid(supplierInfoGrid, PrintContext.DataSource.Settings.Parlor, 4, 1, true);
            SetTextToGrid(supplierInfoGrid, String.Format("DIČ: {0}", PrintContext.DataSource.Supplier.TaxId), 5, 0);
            SetTextToGrid(supplierInfoGrid, String.Format("{0} {1}", PrintContext.DataSource.Settings.Street, PrintContext.DataSource.Settings.Number), 5, 1, true);
            SetTextToGrid(supplierInfoGrid, String.Format("Telefon: {0}", PrintContext.DataSource.Settings.Phone), 6, 0);
            SetTextToGrid(supplierInfoGrid, String.Format("{0} {1}", PrintContext.DataSource.Settings.ZipCode, PrintContext.DataSource.Settings.City), 6, 1, true);
            SetTextToGrid(supplierInfoGrid, String.Format("Mobil: {0}", PrintContext.DataSource.Settings.CellPhone), 7, 0);
            SetTextToGrid(supplierInfoGrid, String.Format("E-mail: {0}", PrintContext.DataSource.Settings.Email), 8, 0);
            SetTextToGrid(supplierInfoGrid, PrintContext.DataSource.Settings.Web, 9, 0);

            SetTextToGrid(supplierInfoGrid, "DODACÍ LIST č.:", 11, 0, true);
            SetTextToGrid(supplierInfoGrid, PrintContext.NoteNumber, 11, 1);
            SetTextToGrid(supplierInfoGrid, "Vystaveno:", 12, 0);
            SetTextToGrid(supplierInfoGrid, PrintContext.NoteDate.ToString("dd.MM.yyyy"), 12, 1);

            Grid subscriberInfoGrid = new Grid
                {
                    ShowGridLines = false,
                    Margin = new Thickness(10, 0, 0, 0)
                };

            subscriberInfoGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(20) });
            subscriberInfoGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(20) });
            subscriberInfoGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(20) });
            subscriberInfoGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(20) });
            subscriberInfoGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(20) });
            subscriberInfoGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(20) });
            subscriberInfoGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(20) });
            subscriberInfoGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(20) });
            subscriberInfoGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(20) });
            subscriberInfoGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(20) });
            subscriberInfoGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(20) });
            subscriberInfoGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(20) });
            subscriberInfoGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(20) });

            subscriberInfoGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(177) });
            subscriberInfoGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(177) });

            SetTextToGrid(subscriberInfoGrid, "Odběratel", 0, 0);
            SetTextToGrid(subscriberInfoGrid, PrintContext.DataSource.Subscriber.Company, 1, 0, true);
            SetTextToGrid(subscriberInfoGrid, String.Format("{0} {1}", PrintContext.DataSource.Subscriber.Street, PrintContext.DataSource.Subscriber.Number), 2, 0, true);
            SetTextToGrid(subscriberInfoGrid, String.Format("{0} {1}", PrintContext.DataSource.Subscriber.ZipCode, PrintContext.DataSource.Subscriber.City), 3, 0, true);
            SetTextToGrid(subscriberInfoGrid, String.Format("IČ: {0}", PrintContext.DataSource.Subscriber.CompanyId), 7, 0);
            SetTextToGrid(subscriberInfoGrid, String.Format("DIČ: {0}", PrintContext.DataSource.Subscriber.TaxId), 8, 0);
            SetTextToGrid(subscriberInfoGrid, "Objednávka č.:", 10, 0);
            SetTextToGrid(subscriberInfoGrid, PrintContext.OrderNumber, 10, 1);
            SetTextToGrid(subscriberInfoGrid, "Datum objednávky: ", 11, 0);
            SetTextToGrid(subscriberInfoGrid, PrintContext.OrderDate.ToString("dd.MM.yyyy"), 11, 1);

            Border subscriberBorder = new Border { BorderThickness = new Thickness(1, 0, 0, 0), BorderBrush = Brushes.Black, Child = subscriberInfoGrid };

            StackPanel infoGridPanel = new StackPanel { Orientation = Orientation.Horizontal };
            infoGridPanel.Children.Add(supplierInfoGrid);
            infoGridPanel.Children.Add(subscriberBorder);

            Grid.SetRow(infoGridPanel, 0);
            grid.Children.Add(infoGridPanel);

            UIElement itemTitlePart = GetItemTitlePart(true);
            grid.Children.Add(itemTitlePart);
            Grid.SetRow(itemTitlePart, 1);

            UIElement itemsPart = GetItemsPart();
            grid.Children.Add(itemsPart);
            Grid.SetRow(itemsPart, 2);

            if (PageCount != 1)
            {
                return;
            }

            UIElement createdByPart = GetCreatedByPart();
            grid.Children.Add(createdByPart);
            Grid.SetRow(createdByPart, 3);

            UIElement stampPart = GetStampPart();
            grid.Children.Add(stampPart);
            Grid.SetRow(stampPart, 4);
        }

        #endregion

        #region Internals and Helpers

        private void SetTextToGrid(Grid grid, string text, int row, int column, bool bold = false)
        {
            TextBlock block = new TextBlock
                {
                    Text = text,
                    FontWeight = bold ? FontWeights.Bold : FontWeights.Normal
                };

            grid.Children.Add(block);
            Grid.SetRow(block, row);
            Grid.SetColumn(block, column);
        }

        #endregion
    }
}