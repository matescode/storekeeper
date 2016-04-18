using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;

using Microsoft.Win32;

using StoreKeeper.Common;

namespace StoreKeeper.Update
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            ConnectionStringTextBox.Text = @"Server=MATESC-PC\SQLSERVER2012;Initial Catalog=StoreKeeper;User ID=StoreKeeperUser;Password=Welcome1;Integrated Security=false;MultipleActiveResultSets=True";
#else
            ConnectionStringTextBox.Text = @"Server=SERVER-SQL\POHODA;Initial Catalog=StoreKeeper;User ID=StoreKeeperUser;Password=Welcome1;Integrated Security=false;MultipleActiveResultSets=True";
#endif
        }

        private void OpenFileButton_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.CheckFileExists = true;
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "SQL Soubory|*.sql";
            if (openFileDialog.ShowDialog() == true)
            {
                FileName.Text = openFileDialog.FileName;
            }
        }

        private void TestButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(ConnectionStringTextBox.Text))
                {
                    throw new ArgumentException("ConnectionString");
                }

                ConnectionStringHolder.Initialize(ConnectionStringTextBox.Text);

                using (StoreKeeperDataContext dataContext = new StoreKeeperDataContext())
                {
                    dataContext.Database.ExecuteSqlCommand("SELECT 1");
                }

                MessageBox.Show("Spojeno!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Chyba spojení: {0}", ex.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                ConnectionStringHolder.Close();
            }
        }

        private void UpdateButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(ConnectionStringTextBox.Text))
                {
                    throw new ArgumentException("ConnectionString");
                }

                string script = File.ReadAllText(FileName.Text, Encoding.UTF8);
                IEnumerable<string> scriptParts = PrepareScript(script);

                ConnectionStringHolder.Initialize(ConnectionStringTextBox.Text);

                using (StoreKeeperDataContext dataContext = new StoreKeeperDataContext())
                {
                    foreach (string part in scriptParts)
                    {
                        dataContext.Database.ExecuteSqlCommand(part);
                    }
                }

                MessageBox.Show("Aktualizováno!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Chyba spojení: {0}", ex.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                ConnectionStringHolder.Close();
            }
        }

        private IEnumerable<string> PrepareScript(string script)
        {
            string[] result = script.Split(new[] {"go\r\n"}, StringSplitOptions.RemoveEmptyEntries);
            return result;
        }
    }
}
