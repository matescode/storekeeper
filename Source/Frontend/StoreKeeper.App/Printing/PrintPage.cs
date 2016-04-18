using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace StoreKeeper.App.Printing
{
    public class PrintPage : IPrintPage
    {
        private readonly int _pageNumber;
        private readonly int _pageCount;
        private readonly IPrintingContext _printingContext;

        public PrintPage(int pageNumber, int pageCount, IPrintingContext printingContext)
        {
            _pageNumber = pageNumber;
            _pageCount = pageCount;
            _printingContext = printingContext;
        }

        #region IPrintPage Implementation

        public UIElement PrintElement
        {
            get { return CreatePrintElement(); }
        }

        #endregion

        #region Internals and Helpers

        private string Label
        {
            get
            {
                if (_pageCount == 1)
                {
                    return _printingContext.Label;
                }
                return String.Format("{0} ({1}/{2})", _printingContext.Label, _pageNumber, _pageCount);
            }
        }

        private UIElement CreatePrintElement()
        {
            TextBlock textBlock = new TextBlock()
            {
                Text = Label,
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(50, 50, 0, 10),
                HorizontalAlignment = HorizontalAlignment.Left
            };

            int recordCount = _printingContext.DataSource.Count;
            int coeficient = _pageNumber - 1;
            List<object> dataSource = _printingContext.DataSource.GetRange(coeficient * PrintManager.RecordsPerPage, Math.Min(PrintManager.RecordsPerPage, recordCount - coeficient * PrintManager.RecordsPerPage));

            DataGrid grid = new DataGrid()
            {
                AutoGenerateColumns = false,
                Margin = new Thickness(50, 10, 30, 10),
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
                HeadersVisibility = DataGridHeadersVisibility.Column,
                BorderBrush = Brushes.White,
                Background = Brushes.White,
                GridLinesVisibility = DataGridGridLinesVisibility.All
            };

            foreach (PrintColumnDefinition columnDef in _printingContext.Columns)
            {
                DataGridTextColumn column = new DataGridTextColumn();
                column.Header = columnDef.Header;
                column.Width = columnDef.Width;
                column.Binding = new Binding(columnDef.BindingProperty);

                if (columnDef.RightAlign)
                {
                    column.ElementStyle = System.Windows.Application.Current.FindResource("RightAlignmentCellStyle") as Style;
                }
                else
                {
                    column.ElementStyle = System.Windows.Application.Current.FindResource("BaseCellStyle") as Style;
                }

                grid.Columns.Add(column);
            }

            StackPanel stack = new StackPanel();
            stack.Children.Add(textBlock);
            stack.Children.Add(grid);
            return stack;
        }

        #endregion
    }
}