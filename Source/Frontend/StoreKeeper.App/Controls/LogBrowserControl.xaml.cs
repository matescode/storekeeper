using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Documents;
using System.Windows.Media;

using CommonBase.Log;
using StoreKeeper.App.Log;

using Microsoft.Win32;

namespace StoreKeeper.App.Controls
{
    /// <summary>
    /// Interaction logic for LogBrowserControl.xaml
    /// </summary>
    public partial class LogBrowserControl
    {
        private const string EntryTypeSeparator = "  @  ";

        public LogBrowserControl()
        {
            InitializeComponent();
        }

        public void SetLogEntries(IEnumerable<LogEntry> logEntries)
        {
            foreach (LogEntry entry in logEntries)
            {
                AppendEntry(entry);
            }
        }

        private void AppendEntry(LogEntry entry)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(FormatStampTime(entry.StampTime));
            sb.Append(EntryTypeSeparator);
            sb.Append(entry.Level.ToString().PadRight(8, ' '));
            sb.Append(EntryTypeSeparator);
            sb.Append(entry.Type);
            sb.Append(EntryTypeSeparator);
            if (entry.Id != -1)
            {
                sb.Append(entry.Id.ToString());
                sb.Append(EntryTypeSeparator);
            }
            sb.Append(entry.Message);

            mainParagraph.Inlines.Add(new Run(sb.ToString()) { Foreground = GetEntryColor(entry.Level) });
            mainParagraph.Inlines.Add(new LineBreak());
        }

        private string FormatStampTime(DateTime stampTime)
        {
            return string.Format(
                "[ {0}-{1}-{2} {3}:{4}:{5} ]",
                stampTime.Year,
                stampTime.Month.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0'),
                stampTime.Day.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0'),
                stampTime.Hour.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0'),
                stampTime.Minute.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0'),
                stampTime.Second.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0'));
        }

        private Brush GetEntryColor(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Debug:
                    return Brushes.DarkGray;

                case LogLevel.Info:
                    return Brushes.Black;

                case LogLevel.Warning:
                    return Brushes.DarkOrange;

                case LogLevel.Critical:
                case LogLevel.Error:
                    return Brushes.Red;

                default:
                    Debug.Assert(false, "Unknown log level");
                    return Brushes.Black;
            }
        }

        public void RemoveContent()
        {
            mainParagraph.Inlines.Clear();
        }

        public bool SaveContent()
        {
            string text = new TextRange(LogTextBox.Document.ContentStart, LogTextBox.Document.ContentEnd).Text;
            if (String.IsNullOrWhiteSpace(text))
            {
                return false;
            }

            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.FileName = "StoreKeeper-AppLog";
            saveDialog.DefaultExt = ".log";
            saveDialog.Filter = "Log document (.log)|*.log|Text document (.txt)|*.txt";
            if (saveDialog.ShowDialog() == true)
            {
                string fileName = saveDialog.FileName;
                using (StreamWriter writer = new StreamWriter(fileName))
                {
                    writer.Write(text);
                }
                return true;
            }
            return false;
        }
    }
}
