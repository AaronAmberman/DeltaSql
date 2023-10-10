using DeltaSql.Properties;
using DeltaSql.Theming;
using System.Windows;

namespace DeltaSql.Services
{
    internal class ThemingService : IThemingService
    {
        #region Fields

        private Theme theme;

        #endregion

        #region Properties

        public Theme Theme 
        { 
            get => theme;
            set 
            {
                theme = value;

                ChangeTheme(value);
            }
        }

        #endregion

        #region Methods

        private void ChangeTheme(Theme theme)
        {
            foreach (ResourceDictionary rd in Application.Current.Resources.MergedDictionaries)
            {
                if (rd is ThemeDictionary td)
                    td.UpdateTheme(theme);
            }
        }

        public void InitializeTheme()
        {
            ServiceLocator.Instance.ThemingService.Theme = (Theme)Settings.Default.Theme;
            ServiceLocator.Instance.MainWindowViewModel.SettingsViewModel.Theme = Settings.Default.Theme;
        }

        public void SetThemeForApp(int theme)
        {
            if (theme != Settings.Default.Theme)
            {
                Settings.Default.Theme = theme;

                ServiceLocator.Instance.ThemingService.Theme = (Theme)theme;
            }
        }

        #endregion
    }
}
