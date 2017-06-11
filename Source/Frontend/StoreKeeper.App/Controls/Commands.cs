using System.Windows.Input;

namespace StoreKeeper.App.Controls
{
    public static class Commands
    {
        public static RoutedCommand ShowLogCommand = new RoutedCommand();

        public static RoutedCommand SyncCommand = new RoutedCommand();

        public static RoutedCommand SyncAllCommand = new RoutedCommand();

        public static RoutedCommand CalculationCommand = new RoutedCommand();

        public static RoutedCommand ShowSettingsCommand = new RoutedCommand();

        public static RoutedCommand SearchCommand = new RoutedCommand();

        public static RoutedCommand ApplicationExitCommand = new RoutedCommand();

        public static RoutedCommand EditStoragesCommand = new RoutedCommand();

        public static RoutedCommand ShowOptionsCommand = new RoutedCommand();

        public static RoutedCommand ShowAboutWindowCommand = new RoutedCommand();

        public static RoutedCommand ShowServerAdministrationCommand = new RoutedCommand();

        public static RoutedCommand StartSearchCommand = new RoutedCommand();

        public static RoutedCommand UpdateCommand = new RoutedCommand();

		public static RoutedCommand DeleteMaterialCommand = new RoutedCommand();

	}
}