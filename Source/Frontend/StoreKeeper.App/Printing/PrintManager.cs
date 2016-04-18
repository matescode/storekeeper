using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Xps;

using CommonBase.UI;
using CommonBase.UI.Localization;

using StoreKeeper.App.Printing.DeliveryNote;

namespace StoreKeeper.App.Printing
{
    public class PrintManager
    {
        internal const int RecordsPerPage = 30;
        private static PrintManager _instance;

        private PrintManager()
        {
        }

        public static void Print(IPrintingContext printContext)
        {
            Instance.PrintDocument(printContext);
        }

        public static void Print(DeliveryNotePrintContext printContext)
        {
            Instance.PrintDeliveryNote(printContext);
        }

        #region Internals and Helpers

        private static PrintManager Instance
        {
            get { return _instance ?? (_instance = new PrintManager()); }
        }

        private void PrintDocument(IPrintingContext printContext)
        {
            List<PrintPage> pages = GetPages(printContext);
            if (pages.Count == 0)
            {
                UIApplication.MessageDialogs.Info("NoPages".Localize());
                return;
            }

            PrintPages(pages, PageOrientation.Landscape, "Material");
        }

        private void PrintDeliveryNote(DeliveryNotePrintContext printContext)
        {
            printContext.CreatePages();
            if (!printContext.IsValid)
            {
                UIApplication.MessageDialogs.Info("NoPages".Localize());
                return;
            }

            PrintPages(printContext.Pages, PageOrientation.Portrait, "List");
        }

        private void PrintPages(IEnumerable<IPrintPage> pages, PageOrientation pageOrientation, string documentLabel)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                FixedDocument fixedDocument = new FixedDocument();
                fixedDocument.Name = documentLabel;
                foreach (IPrintPage page in pages)
                {
                    PageContent pageContent = new PageContent();
                    FixedPage fixedPage = new FixedPage();

                    fixedPage.Children.Add(page.PrintElement);
                    (pageContent as System.Windows.Markup.IAddChild).AddChild(fixedPage);
                    fixedDocument.Pages.Add(pageContent);
                }

                XpsDocumentWriter writer = PrintQueue.CreateXpsDocumentWriter(printDialog.PrintQueue);
                printDialog.PrintTicket.PageOrientation = pageOrientation;
                fixedDocument.PrintTicket = printDialog.PrintTicket;
                writer.Write(fixedDocument, printDialog.PrintTicket);

                UIApplication.MessageDialogs.Info("SentToPrinter".Localize());
            }
        }

        private List<PrintPage> GetPages(IPrintingContext printContext)
        {
            int recordCount = printContext.DataSource.Count;
            int pageCount = recordCount / RecordsPerPage + ((recordCount % RecordsPerPage == 0) ? 0 : 1);

            List<PrintPage> pages = new List<PrintPage>();

            for (int i = 0; i < pageCount; ++i)
            {
                pages.Add(new PrintPage(i + 1, pageCount, printContext));
            }
            return pages;
        }

        #endregion
    }
}