using DeltaSql.Cryptography;
using DeltaSql.Properties;
using DeltaSql.Services;
using DeltaSql.Theming;
using DeltaSql.ViewModels;
using SimpleLogger;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using WPF.Translations;

namespace DeltaSql
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            #region Service Initialization

            ServiceLocator.Instance.Cryptographer = new SimpleCryptographer();
            ServiceLocator.Instance.LoggingService = new LoggingService(new Logger());
            ServiceLocator.Instance.ThemingService = new ThemingService();

            #endregion

            #region Log File

            try
            {
                if (!string.IsNullOrWhiteSpace(Settings.Default.LogPath))
                {
                    // the setting will be the path only (will not include the filename)
                    ServiceLocator.Instance.LoggingService.Logger.LogFile = Path.Combine(Settings.Default.LogPath, "DeltaSql.log");
                }
                else
                {
                    string location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                    if (!string.IsNullOrWhiteSpace(location))
                    {
                        ServiceLocator.Instance.LoggingService.Logger.LogFile = Path.Combine(location, "DeltaSql.log");
                    }
                }
            }
            catch
            {
                // we cannot determine location for some reason, use desktop
                ServiceLocator.Instance.LoggingService.Logger.LogFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "DeltaSql.log");
            }

            #endregion

            #region View Models

            MainWindowViewModel mainWindowViewModel = new MainWindowViewModel
            {
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

            ServiceLocator.Instance.MainWindowViewModel = mainWindowViewModel;
            ServiceLocator.Instance.LoggingService.LogEntry += mainWindowViewModel.LoggingService_LogEntry;

            #endregion

            #region Version

            ServiceLocator.Instance.MainWindowViewModel.Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            #endregion

            #region Translations

            // set settings
            if (string.IsNullOrWhiteSpace(Settings.Default.Language))
            {
                // load english if the language is missing from settings
                Settings.Default.Language = "en";
                Settings.Default.Save();
            }

            // set our culture to the current one from settings
            Thread.CurrentThread.CurrentCulture = new CultureInfo(Settings.Default.Language);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(Settings.Default.Language);

            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo(Settings.Default.Language);

            // set translations
            ServiceLocator.Instance.MainWindowViewModel.Translations = new Translation(new ResourceDictionary
            {
                Source = new Uri($"pack://application:,,,/Translations/Translations.{Settings.Default.Language}.xaml")
            }, new ResourceDictionaryTranslationDataProvider(), false);

            // need translations for view model
            mainWindowViewModel.SettingsViewModel = new SettingsViewModel();
            mainWindowViewModel.SettingsViewModel.SetLanguage(Settings.Default.Language);

            #endregion

            #region Theming

            ServiceLocator.Instance.ThemingService.Theme = (Theme)Settings.Default.Theme;

            mainWindowViewModel.SettingsViewModel.Theme = Settings.Default.Theme;

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

                MessageBox.Show(ServiceLocator.Instance.MainWindowViewModel?.Translations.UnhandledErrorMessage ?? "Unhandled exception occurred. We have logged the issue.",
                    ServiceLocator.Instance.MainWindowViewModel?.Translations.UnhandledErrorTitle ?? "Unhandled Exception", 
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
