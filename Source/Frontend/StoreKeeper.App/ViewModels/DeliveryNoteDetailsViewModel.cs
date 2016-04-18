using System;
using StoreKeeper.App.Printing.DeliveryNote;

namespace StoreKeeper.App.ViewModels
{
    public class DeliveryNoteDetailsViewModel : ViewModelBase
    {
        private readonly DeliveryNotePrintContext _printContext;

        public DeliveryNoteDetailsViewModel(DeliveryNotePrintContext printContext)
        {
            _printContext = printContext;
        }

        #region Properties

        public string NoteNumber
        {
            get { return _printContext.NoteNumber; }
            set
            {
                _printContext.NoteNumber = value;
                NotifyPropertyChanged("NoteNumber");
            }
        }

        public DateTime NoteDate
        {
            get { return _printContext.NoteDate; }
            set
            {
                _printContext.NoteDate = value;
                NotifyPropertyChanged("NoteDate");
            }
        }

        public string OrderNumber
        {
            get { return _printContext.OrderNumber; }
            set
            {
                _printContext.OrderNumber = value;
                NotifyPropertyChanged("OrderNumber");
            }
        }

        public DateTime OrderDate
        {
            get { return _printContext.OrderDate; }
            set
            {
                _printContext.OrderDate = value;
                NotifyPropertyChanged("OrderDate");
            }
        }

        #endregion
    }
}