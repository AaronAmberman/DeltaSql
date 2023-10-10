using DeltaSql.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Windows;
using WPF.Translations;

namespace DeltaSql.Services
{
    internal class TranslationService : ITranslationService
    {
        #region Constants

        public const string ENGLISH_CULTURE = "en";
        public const string CHINESE_CULTURE = "zh-Hans";
        public const string FRENCH_CULTURE = "fr";
        public const string GERMAN_CULTURE = "de";
        public const string ITALIAN_CULTURE = "it";
        public const string JAPANESE_CULTURE = "ja";
        public const string KOREAN_CULTURE = "ko";
        public const string NORWEGIAN_CULTURE = "no";
        public const string PORTUGUESE_CULTURE = "pt";
        public const string RUSSIAN_CULTURE = "ru";
        public const string SPANISH_CULTURE = "es";

        #endregion

        #region Fields

        private Translation translations;

        #endregion

        #region Properties

        public dynamic Translations 
        { 
            get => translations;
            set
            {
                translations = value;

                ServiceLocator.Instance.MainWindowViewModel.Translations = translations;
            }
        }

        public IDictionary<string, string> Languages => new Dictionary<string, string>
        {
            { ENGLISH_CULTURE, Translations.English },
            { CHINESE_CULTURE, Translations.Chinese },
            { FRENCH_CULTURE, Translations.French },
            { GERMAN_CULTURE, Translations.German },
            { ITALIAN_CULTURE, Translations.Italian },
            { JAPANESE_CULTURE, Translations.Japanese },
            { KOREAN_CULTURE, Translations.Korean },
            { NORWEGIAN_CULTURE, Translations.Norwegian },
            { PORTUGUESE_CULTURE, Translations.Portuguese },
            { RUSSIAN_CULTURE, Translations.Russian },
            { SPANISH_CULTURE, Translations.Spanish }
        };

        #endregion

        #region Methods

        public void SetThreadCultureAndTranslations(string culture, bool setSettings)
        {
            if (setSettings)
                Settings.Default.Language = culture;

            Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);

            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo(Settings.Default.Language);

            Translations = new Translation(new ResourceDictionary
            {
                Source = new Uri($"pack://application:,,,/Translations/Translations.{Settings.Default.Language}.xaml")
            }, new ResourceDictionaryTranslationDataProvider(), false);
        }

        #endregion
    }
}
