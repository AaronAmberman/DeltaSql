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

            ServiceLocator.Instance.MainWindowViewModel.RichTextBox = output;

            ServiceLocator.Instance.LoggingService.Debug(ServiceLocator.Instance.TranslationService.Translations.AwaitingConnection1);
            ServiceLocator.Instance.LoggingService.Debug(ServiceLocator.Instance.TranslationService.Translations.AwaitingConnection2);
        }
    }
}
