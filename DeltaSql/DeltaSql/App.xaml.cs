using DeltaSql.Properties;
using DeltaSql.Services;
using DeltaSql.ViewModels;
using SimpleLogger;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;

namespace DeltaSql
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            #region Service Initialization

            ServiceLocator.Instance.Cryptographer = new CryptographyService();
            ServiceLocator.Instance.LoggingService = new LoggingService(new Logger());
            ServiceLocator.Instance.PreviousConnectionsService = new PreviousConnectionsService();
            ServiceLocator.Instance.ThemingService = new ThemingService();
            ServiceLocator.Instance.TranslationService = new TranslationService();

            #endregion

            #region Log File

            ServiceLocator.Instance.LoggingService.SetLogPath();

            #endregion

            #region View Models

            MainWindowViewModel mainWindowViewModel = new MainWindowViewModel
            {
                ConnectionViewModelLeft = new ConnectionViewModel(),
                ConnectionViewModelRight = new ConnectionViewModel(),
                MessageBoxViewModel = new MessageBoxViewModel(),
                ProgressViewModel = new ProgressViewModel(),
                SqlInputViewModelLeft = new SqlInputViewModel(),
                SqlInputViewModelRight = new SqlInputViewModel()
            };
            mainWindowViewModel.SqlInputViewModelLeft.Connected += mainWindowViewModel.SqlInputViewModel_Connected;
            mainWindowViewModel.SqlInputViewModelLeft.Connecting += mainWindowViewModel.SqlInputViewModel_Connecting;
            mainWindowViewModel.SqlInputViewModelLeft.Disconnected += mainWindowViewModel.SqlInputViewModel_Disconnected;
            mainWindowViewModel.SqlInputViewModelRight.Connected += mainWindowViewModel.SqlInputViewModel_Connected;
            mainWindowViewModel.SqlInputViewModelRight.Connecting += mainWindowViewModel.SqlInputViewModel_Connecting;
            mainWindowViewModel.SqlInputViewModelRight.Disconnected += mainWindowViewModel.SqlInputViewModel_Disconnected;

            ServiceLocator.Instance.LoggingService.LogEntry += mainWindowViewModel.LoggingService_LogEntry;            
            ServiceLocator.Instance.MainWindowViewModel = mainWindowViewModel;

            #endregion

            #region Version

            try
            {
                ServiceLocator.Instance.MainWindowViewModel.Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
            catch (Exception ex)
            {
                ServiceLocator.Instance.LoggingService.Logger.Error($"An error occurred attempting to get the version: {ex.Message}");

                ServiceLocator.Instance.MainWindowViewModel.Version = "0.0.0.0";
            }

            #endregion

            #region Translations

            // set settings
            if (string.IsNullOrWhiteSpace(Settings.Default.Language))
            {
                // load english if the language is missing from settings
                Settings.Default.Language = TranslationService.ENGLISH_CULTURE;
                Settings.Default.Save();
            }

            ServiceLocator.Instance.TranslationService.SetThreadCultureAndTranslations(Settings.Default.Language, false);

            // need translations for view model
            mainWindowViewModel.SettingsViewModel = new SettingsViewModel();
            mainWindowViewModel.SettingsViewModel.SetLanguage(Settings.Default.Language);

            #endregion

            #region Theming

            ServiceLocator.Instance.ThemingService.InitializeTheme();

            #endregion

            #region Settings

            ServiceLocator.Instance.PreviousConnectionsService.Initialize();

            #endregion
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {

        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                ServiceLocator.Instance.LoggingService.Logger.Fatal($"An unhandled exception occurred. Details:{Environment.NewLine}{e.Exception}");

                MessageBox.Show(ServiceLocator.Instance.TranslationService?.Translations.UnhandledErrorMessage ?? "Unhandled exception occurred. We have logged the issue.",
                    ServiceLocator.Instance.TranslationService?.Translations.UnhandledErrorTitle ?? "Unhandled Exception", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An error occurred ensuring the log file exists or an error occurred trying to write to the log file.{Environment.NewLine}{ex}");
            }

            Environment.Exit(e.Exception.HResult);
        }
    }
}
