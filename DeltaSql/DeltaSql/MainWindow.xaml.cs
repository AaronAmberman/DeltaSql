using DeltaSql.Theming;
using System.Windows;
using System.Windows.Controls;

namespace DeltaSql
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            ServiceLocator.Instance.MainWindowViewModel.Dispatcher = Dispatcher;

            DataContext = ServiceLocator.Instance.MainWindowViewModel;

            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ServiceLocator.Instance.MainWindowViewModel.ShowMessageBox("Hello");
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ServiceLocator.Instance.ThemingService.Theme = (Theme)ThemeCB.SelectedIndex;
        }
    }
}
