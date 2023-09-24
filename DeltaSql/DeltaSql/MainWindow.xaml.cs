using System.Windows;

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
    }
}
