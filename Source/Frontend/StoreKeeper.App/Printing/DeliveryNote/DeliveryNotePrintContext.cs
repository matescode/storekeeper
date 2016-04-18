using System;
using System.Collections.Generic;

namespace StoreKeeper.App.Printing.DeliveryNote
{
    public class DeliveryNotePrintContext
    {
        private const int OnePageItemCount = 20;
        private const int FirstPageItemCount = 35;
        private const int NextPageItemCount = 50;
        private const int LastPageItemCount = 40;

        private readonly IDeliveryNoteDataSource _dataSource;
        private readonly List<DeliveryNotePageBase> _pages;
        private readonly List<List<IDeliveryNoteItem>> _pagesData;

        public DeliveryNotePrintContext(IDeliveryNoteDataSource dataSource)
        {
            _dataSource = dataSource;
            _pages = new List<DeliveryNotePageBase>();
            _pagesData = new List<List<IDeliveryNoteItem>>();
        }

        #region Properties

        public bool IsValid
        {
            get { return _pages.Count > 0; }
        }

        public IEnumerable<DeliveryNotePageBase> Pages
        {
            get { return _pages; }
        }

        public string CreatedBy
        {
            get { return AppContext.Config.DeliveryNoteCreatedByName; }
        }

        public string CreatedByMail
        {
            get { return AppContext.Config.DeliveryNoteCreatedByMail; }
        }

        public IDeliveryNoteDataSource DataSource
        {
            get { return _dataSource; }
        }

        public string NoteNumber { get; set; }

        public DateTime NoteDate { get; set; }

        public string OrderNumber { get; set; }

        public DateTime OrderDate { get; set; }

        #endregion

        #region Public Methods

        public void CreatePages()
        {
            CreatePagesData();
            int pageCount = _pagesData.Count;
            for (int i = 1; i <= pageCount; ++i)
            {
                if (i == 1)
                {
                    _pages.Add(new DeliveryNoteFirstPage(this, pageCount));
                    continue;
                }

                if (i == pageCount)
                {
                    _pages.Add(new DeliveryNoteLastPage(this, i, pageCount));
                    continue;
                }

                _pages.Add(new DeliveryNoteMiddlePage(this, i, pageCount));
            }
        }

        public IEnumerable<IDeliveryNoteItem> GetPageItems(int pageNumber)
        {
            return _pagesData[pageNumber - 1];
        }

        #endregion

        #region Internals and Helpers

        private void CreatePagesData()
        {
            _pagesData.Clear();
            if (_dataSource.Items.Count <= OnePageItemCount)
            {
                _pagesData.Add(_dataSource.Items);
                return;
            }

            if (_dataSource.Items.Count <= FirstPageItemCount)
            {
                _pagesData.Add(_dataSource.Items);
                _pagesData.Add(new List<IDeliveryNoteItem>());
                return;
            }

            int count = _dataSource.Items.Count;

            _pagesData.Add(_dataSource.Items.GetRange(0, FirstPageItemCount));
            count -= FirstPageItemCount;
            int lastIndex = FirstPageItemCount;
            while (count > NextPageItemCount)
            {
                _pagesData.Add(_dataSource.Items.GetRange(lastIndex, NextPageItemCount));
                lastIndex += NextPageItemCount;
                count -= NextPageItemCount;
            }

            if (count > 0)
            {
                _pagesData.Add(_dataSource.Items.GetRange(lastIndex, count));
                if (count > LastPageItemCount)
                {
                    _pagesData.Add(new List<IDeliveryNoteItem>());
                }
            }
        }

        #endregion
    }
}