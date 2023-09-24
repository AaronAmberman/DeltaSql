using System.Windows;

namespace DeltaSql
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DataContext = ServiceLocator.Instance.MainWindowViewModel;

            InitializeComponent();
        }
    }
}
