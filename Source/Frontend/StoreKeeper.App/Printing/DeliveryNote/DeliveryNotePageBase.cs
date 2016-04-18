using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace StoreKeeper.App.Printing.DeliveryNote
{
    public abstract class DeliveryNotePageBase : IPrintPage
    {
        protected DeliveryNotePageBase(DeliveryNotePrintContext printContext, int currentPage, int pageCount)
        {
            PrintContext = printContext;
            CurrentPage = currentPage;
            PageCount = pageCount;
        }

        #region IPrintPage Implementation

        public UIElement PrintElement
        {
            get { return CreatePageElement(); }
        }

        #endregion

        #region Properties

        public DeliveryNotePrintContext PrintContext { get; private set; }

        public int PageCount { get; private set; }

        public int CurrentPage { get; private set; }

        #endregion

        #region Abstract Properties and Methods

        protected abstract void CreateGridContent(Grid grid);

        #endregion

        #region Internals and Helpers

        private UIElement CreatePageElement()
        {
            Grid pageGrid = new Grid
                {
                    Width = 710,
                    Height = 960,
                };

            Border gridBorder = new Border
                {
                    Child = pageGrid,
                    BorderThickness = new Thickness(0.5),
                    BorderBrush = Brushes.Black
                };

            CreateGridContent(pageGrid);

            StackPanel titlePanel = new StackPanel
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Margin = new Thickness(0, 50, 0, 0),
                    Orientation = Orientation.Horizontal,
                };

            titlePanel.Children.Add(
                new TextBlock
                {
                    Text = "FLAJZAR, s.r.o.",
                    Width = 370,
                    FontSize = 16,
                    FontWeight = FontWeights.Bold
                });

            titlePanel.Children.Add(
                new TextBlock
                {
                    Text = SecondLabel,
                    Width = 370,
                    TextAlignment = TextAlignment.Right,
                    FontSize = 16,
                    FontWeight = FontWeights.Bold
                });

            StackPanel pageContainer = new StackPanel
                {
                    Margin = new Thickness(25, 0, 25, 0)
                };
            pageContainer.Children.Add(titlePanel);
            pageContainer.Children.Add(gridBorder);

            return pageContainer;
        }

        private string SecondLabel
        {
            get
            {
                if (PageCount == 1)
                {
                    return "Dodací list";
                }

                return String.Format("Dodací list {0}/{1}", CurrentPage, PageCount);
            }
        }

        protected UIElement GetCreatedByPart()
        {
            StackPanel createdPanel = new StackPanel { Orientation = Orientation.Horizontal, VerticalAlignment = VerticalAlignment.Center };
            createdPanel.Children.Add(
                new TextBlock
                {
                    Text = "Vystavil:",
                    Width = 160,
                    Margin = new Thickness(20, 0, 0, 0)
                });

            StackPanel createdContactPanel = new StackPanel();
            createdContactPanel.Children.Add(
                new TextBlock
                {
                    Text = PrintContext.CreatedBy
                });
            createdContactPanel.Children.Add(
                new TextBlock
                {
                    Text = PrintContext.CreatedByMail
                });

            createdPanel.Children.Add(createdContactPanel);

            Border createdPanelBorder = new Border
            {
                Child = createdPanel,
                BorderThickness = new Thickness(0, 0.5, 0, 0.5),
                BorderBrush = Brushes.Black,
            };

            return createdPanelBorder;
        }

        protected UIElement GetStampPart()
        {
            StackPanel stampPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal, 
                    VerticalAlignment = VerticalAlignment.Top, 
                    Margin = new Thickness(0, 10, 0, 0)
                };
            
            stampPanel.Children.Add(
                new TextBlock
                {
                    Text = "Převzal:",
                    Width = 420,
                    Margin = new Thickness(20, 0, 0, 0)
                });

            stampPanel.Children.Add(
                new TextBlock
                {
                    Text = "Razítko:"
                });

            return stampPanel;
        }

        protected UIElement GetItemTitlePart(bool topVisible)
        {
            StackPanel itemTitlePanel = new StackPanel { Orientation = Orientation.Horizontal, VerticalAlignment = VerticalAlignment.Center };
            itemTitlePanel.Children.Add(
                new TextBlock
                {
                    Text = "Označení",
                    Width = 100,
                    Margin = new Thickness(10, 0, 0, 0)
                });

            itemTitlePanel.Children.Add(
                new TextBlock
                {
                    Text = "Kód",
                    Width = 100,
                });

            itemTitlePanel.Children.Add(
                new TextBlock
                {
                    Text = "Název",
                    Width = 400,
                });

            itemTitlePanel.Children.Add(
                new TextBlock
                {
                    Text = "Množství",
                    Width = 80,
                });

            Border itemTitleBorder = new Border
            {
                Child = itemTitlePanel,
                BorderThickness = new Thickness(0, topVisible ? 0.5 : 0, 0, 0.5),
                BorderBrush = Brushes.Black,
            };

            return itemTitleBorder;
        }

        protected UIElement GetItemsPart()
        {
            IEnumerable<IDeliveryNoteItem> dataSource = PrintContext.GetPageItems(CurrentPage);

            DataGrid grid = new DataGrid()
            {
                AutoGenerateColumns = false,
                Margin = new Thickness(5, 0, 0, 0),
                SelectionUnit = DataGridSelectionUnit.CellOrRowHeader,
                SelectionMode = DataGridSelectionMode.Single,
                CanUserResizeColumns = false,
                CanUserSortColumns = false,
                CanUserAddRows = false,
                CanUserDeleteRows = false,
                CanUserReorderColumns = false,
                EnableRowVirtualization = false,
                ItemsSource = dataSource,
                IsReadOnly = true,
                HeadersVisibility = DataGridHeadersVisibility.None,
                BorderBrush = Brushes.White,
                Background = Brushes.White,
                GridLinesVisibility = DataGridGridLinesVisibility.Vertical
            };

            DataGridTextColumn column = new DataGridTextColumn();
            column.Width = 100;
            column.Binding = new Binding("SpecialCode");
            grid.Columns.Add(column);

            column = new DataGridTextColumn();
            column.Width = 100;
            column.Binding = new Binding("CentralCode");
            grid.Columns.Add(column);

            column = new DataGridTextColumn();
            column.Width = 400;
            column.Binding = new Binding("Name");
            grid.Columns.Add(column);

            column = new DataGridTextColumn();
            column.Width = 80;
            column.Binding = new Binding("Amount");
            
            Style cellStyle = new Style(typeof(TextBlock));
            cellStyle.Setters.Add(new Setter(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Right));
            column.ElementStyle = cellStyle;

            grid.Columns.Add(column);

            return grid;
        }

        #endregion
    }
}