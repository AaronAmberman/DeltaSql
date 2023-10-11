using DeltaSql.Properties;
using DeltaSql.Services;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace DeltaSql.ViewModels
{
    internal class SettingsViewModel : ViewModelBase
    {
        #region Fields

        private ICommand aboutCommand;
        private Visibility aboutBoxVisibility = Visibility.Collapsed;
        private ICommand browseLogCommand;
        private ICommand cancelCommand;
        private string logFile;
        private ICommand okCommand;
        private MessageBoxResult result;
        private KeyValuePair<string, string> selectedLanguage;
        private int theme;
        private Visibility visibility = Visibility.Collapsed;

        #endregion

        #region Properties

        public ICommand AboutCommand => aboutCommand ??= new RelayCommand(About);

        public Visibility AboutBoxVisibility
        {
            get => aboutBoxVisibility;
            set
            {
                aboutBoxVisibility = value;
                OnPropertyChanged();
            }
        }

        public ICommand BrowseLogCommand => browseLogCommand ??= new RelayCommand(BrowseLog);

        public ICommand CancelCommand => cancelCommand ??= new RelayCommand(Cancel);

        public IDictionary<string, string> Languages => ServiceLocator.Instance.TranslationService.Languages;

        public string LogPath
        {
            get => logFile;
            set
            {
                logFile = value;
                OnPropertyChanged();
            }
        }

        public ICommand OkCommand => okCommand ??= new RelayCommand(Ok);

        public MessageBoxResult Result
        {
            get => result;
            set
            {
                result = value;
                OnPropertyChanged();
            }
        }

        public KeyValuePair<string, string> SelectedLanguage
        {
            get => selectedLanguage;
            set
            {
                selectedLanguage = value;
                OnPropertyChanged();
            }
        }

        public int Theme
        {
            get => theme;
            set
            {
                theme = value;
                OnPropertyChanged();
            }
        }

        public Visibility Visibility
        {
            get => visibility;
            set
            {
                visibility = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Methods

        private void About()
        {
            AboutBoxVisibility = Visibility.Visible;
        }

        private void BrowseLog()
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                AddExtension = true,
                CheckPathExists = true,                
                FileName = "DeltaSql.log",
                Filter = "Text Files(*.txt)|*.txt|Log Files(*.log)|*.log",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Multiselect = false,
                Title = ServiceLocator.Instance.TranslationService.Translations.BrowseTitle,
                ValidateNames = true
            };

            bool? result = ofd.ShowDialog();

            if (!result.HasValue) return;
            if (!result.Value) return;

            string file = ofd.FileName;

            LogPath = Path.GetDirectoryName(file);
        }

        private void Cancel()
        {
            LogPath = Settings.Default.LogPath;

            Result = MessageBoxResult.Cancel;
            Visibility = Visibility.Collapsed;
        }

        private void Ok()
        {
            if (SelectedLanguage.Key != Settings.Default.Language)
                ServiceLocator.Instance.TranslationService.SetThreadCultureAndTranslations(SelectedLanguage.Key, true);

            ServiceLocator.Instance.LoggingService.SetLogPath(LogPath);
            Settings.Default.LogPath = LogPath;

            ServiceLocator.Instance.ThemingService.SetThemeForApp(Theme);

            Settings.Default.Save();

            Result = MessageBoxResult.OK;
            Visibility = Visibility.Collapsed;
        }

        public void SetLanguage(string language)
        {
            switch (language) 
            {
                case TranslationService.ENGLISH_CULTURE: SelectedLanguage = new KeyValuePair<string, string>(language, ServiceLocator.Instance.TranslationService.Translations.English); break;
                case TranslationService.CHINESE_CULTURE: SelectedLanguage = new KeyValuePair<string, string>(language, ServiceLocator.Instance.TranslationService.Translations.Chinese); break;
                case TranslationService.FRENCH_CULTURE: SelectedLanguage = new KeyValuePair<string, string>(language, ServiceLocator.Instance.TranslationService.Translations.French); break;
                case TranslationService.GERMAN_CULTURE: SelectedLanguage = new KeyValuePair<string, string>(language, ServiceLocator.Instance.TranslationService.Translations.German); break;
                case TranslationService.ITALIAN_CULTURE: SelectedLanguage = new KeyValuePair<string, string>(language, ServiceLocator.Instance.TranslationService.Translations.Italian); break;
                case TranslationService.JAPANESE_CULTURE: SelectedLanguage = new KeyValuePair<string, string>(language, ServiceLocator.Instance.TranslationService.Translations.Japanese); break;
                case TranslationService.KOREAN_CULTURE: SelectedLanguage = new KeyValuePair<string, string>(language, ServiceLocator.Instance.TranslationService.Translations.Korean); break;
                case TranslationService.NORWEGIAN_CULTURE: SelectedLanguage = new KeyValuePair<string, string>(language, ServiceLocator.Instance.TranslationService.Translations.Norwegian); break;
                case TranslationService.PORTUGUESE_CULTURE: SelectedLanguage = new KeyValuePair<string, string>(language, ServiceLocator.Instance.TranslationService.Translations.Portuguese); break;
                case TranslationService.RUSSIAN_CULTURE: SelectedLanguage = new KeyValuePair<string, string>(language, ServiceLocator.Instance.TranslationService.Translations.Russian); break;
                case TranslationService.SPANISH_CULTURE: SelectedLanguage = new KeyValuePair<string, string>(language, ServiceLocator.Instance.TranslationService.Translations.Spanish); break;
                default: SelectedLanguage = new KeyValuePair<string, string>(language, ServiceLocator.Instance.TranslationService.Translations.English); break;
            }
        }

        #endregion
    }
}
