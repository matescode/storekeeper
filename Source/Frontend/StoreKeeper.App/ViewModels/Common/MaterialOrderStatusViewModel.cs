using System;
using System.Windows.Media;
using CommonBase.Resources;
using CommonBase.UI.Localization;
using StoreKeeper.Client.Objects;

namespace StoreKeeper.App.ViewModels.Common
{
    public class MaterialOrderStatusViewModel
    {
        private readonly IMaterialOrderStatus _materialOrderStatus;

        public MaterialOrderStatusViewModel(IMaterialOrderStatus materialOrderStatus)
        {
            _materialOrderStatus = materialOrderStatus;
        }

        public string OrderedStr
        {
            get
            {
                if (_materialOrderStatus.AccountingOrderStatus == AccountingOrderStatus.NotNecessary)
                {
                    return String.Empty;
                }

                if (_materialOrderStatus.AccountingOrderStatus == AccountingOrderStatus.NotOrdered)
                {
                    return CommonStrings.No.Localize();
                }
                return String.Format("{0} ({1})", CommonStrings.Yes.Localize(), _materialOrderStatus.AccountingOrderCount);
            }
        }

        public Brush OrderedColorBrush
        {
            get
            {
                switch (_materialOrderStatus.AccountingOrderStatus)
                {
                    case AccountingOrderStatus.NotNecessary:
                        return Brushes.Black;

                    case AccountingOrderStatus.NotOrdered:
                        return Brushes.Red;

                    case AccountingOrderStatus.Ordered:
                        return Brushes.DarkOrange;

                    case AccountingOrderStatus.OrderedCompletely:
                        return Brushes.ForestGreen;

                    default:
                        return Brushes.Black;
                }
            }
        }
    }
}