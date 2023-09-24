using DeltaSql.Properties;
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

            ServiceLocator.Instance.Logger = new Logger();

            #endregion

            #region Log File

            try
            {
                if (!string.IsNullOrWhiteSpace(Settings.Default.LogPath))
                {
                    // the setting will be the path only (will not include the filename)
                    ServiceLocator.Instance.Logger.LogFile = Path.Combine(Settings.Default.LogPath, "DeltaSql.log");
                }
                else
                {
                    string location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                    if (!string.IsNullOrWhiteSpace(location))
                    {
                        ServiceLocator.Instance.Logger.LogFile = Path.Combine(location, "DeltaSql.log");
                    }
                }
            }
            catch
            {
                // we cannot determine location for some reason, use desktop
                ServiceLocator.Instance.Logger.LogFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "DeltaSql.log");
            }

            #endregion

            #region View Models

            MainWindowViewModel mainWindowViewModel = new MainWindowViewModel
            {
                MessageBoxViewModel = new MessageBoxViewModel(),
                ProgressViewModel = new ProgressViewModel()
            };

            ServiceLocator.Instance.MainWindowViewModel = mainWindowViewModel;

            #endregion

            #region Version

            ServiceLocator.Instance.MainWindowViewModel.Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            #endregion

            #region Translations

            // set settings
            if (string.IsNullOrWhiteSpace(Settings.Default.Language))
            {
                // load english if the language is missing from settings
                Settings.Default.Language = "en-US";
                Settings.Default.Save();
            }

            // set our culture to the current one from settings
            Thread.CurrentThread.CurrentCulture = new CultureInfo(Settings.Default.Language);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(Settings.Default.Language);

            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo(Settings.Default.Language);

            // set translations
            Translation translation = new Translation(new ResourceDictionary 
            { 
                Source = new Uri($"pack://application:,,,/Translations/Translations.{Settings.Default.Language}.xaml") 
            }, new ResourceDictionaryTranslationDataProvider(), false);

            ServiceLocator.Instance.MainWindowViewModel.Translations = translation;

            #endregion

            #region Theming



            #endregion
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {

        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                ServiceLocator.Instance.Logger.Error($"An unhandled exception occurred. Details:{Environment.NewLine}{e.Exception}");

                MessageBox.Show("Unhandled exception occurred. We have logged the issue.",
                    "Unhandled Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An error occurred ensuring the log file exists or an error occurred trying to write to the log file.{Environment.NewLine}{ex}");
            }

            Environment.Exit(e.Exception.HResult);
        }
    }
}
