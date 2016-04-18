using System;
using System.Text;
using System.Windows;
using StoreKeeper.Common;

namespace StoreKeeper.ConnectionTester
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(ConnectionStringTextBox.Text))
                {
                    throw new ArgumentException("ConnectionString");
                }

                ConnectionStringHolder.Initialize(ConnectionStringTextBox.Text);
                StringBuilder sb = new StringBuilder("Spojeno:\n");

                using (StoreKeeperDataContext dataContext = new StoreKeeperDataContext())
                {
                    dataContext.Database.ExecuteSqlCommand("SELECT 1");
                    sb.AppendFormat("Poslední aktualizace: {0}", dataContext.LastUpdate);
                }

                MessageBox.Show(sb.ToString(), "Info", MessageBoxButton.OK, MessageBoxImage.Information);
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

        private void TestLockButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(ConnectionStringTextBox.Text))
                {
                    throw new ArgumentException("ConnectionString");
                }

                ConnectionStringHolder.Initialize(ConnectionStringTextBox.Text);
                StringBuilder sb = new StringBuilder("Spojeno:\n");

                using (StoreKeeperDataContext dataContext = new StoreKeeperDataContext())
                {
                    sb.AppendFormat("LockedBy: {0}\n{1}", dataContext.LockedBy, dataContext.LockedByUser);
                }

                MessageBox.Show(sb.ToString(), "Info", MessageBoxButton.OK, MessageBoxImage.Information);
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

        private void ClearLockButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(ConnectionStringTextBox.Text))
                {
                    throw new ArgumentException("ConnectionString");
                }

                ConnectionStringHolder.Initialize(ConnectionStringTextBox.Text);
                StringBuilder sb = new StringBuilder("OK");

                using (StoreKeeperDataContext dataContext = new StoreKeeperDataContext())
                {
                    dataContext.Database.ExecuteSqlCommand("UPDATE SystemInformations SET Value='' where Name='LockedBy'");
                }

                MessageBox.Show("OK", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
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
    }
}
