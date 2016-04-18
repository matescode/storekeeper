using System.Windows;
using System.Windows.Controls;

namespace StoreKeeper.App.Printing.DeliveryNote
{
    public class DeliveryNoteLastPage : DeliveryNotePageBase
    {
        public DeliveryNoteLastPage(DeliveryNotePrintContext printContext, int currentPage, int pageCount)
            : base(printContext, currentPage, pageCount)
        {
        }

        #region Overrides

        protected override void CreateGridContent(Grid grid)
        {
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(100) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(100) });

            UIElement itemTitlePart = GetItemTitlePart(false);
            grid.Children.Add(itemTitlePart);
            Grid.SetRow(itemTitlePart, 0);

            UIElement itemsPart = GetItemsPart();
            grid.Children.Add(itemsPart);
            Grid.SetRow(itemsPart, 1);

            UIElement createdByPart = GetCreatedByPart();
            grid.Children.Add(createdByPart);
            Grid.SetRow(createdByPart, 2);

            UIElement stampPart = GetStampPart();
            grid.Children.Add(stampPart);
            Grid.SetRow(stampPart, 3);
        }

        #endregion
    }
}