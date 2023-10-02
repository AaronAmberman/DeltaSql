using System;
using System.Windows;

namespace DeltaSql.Theming
{
    internal class ThemeDictionary : ResourceDictionary
    {
        #region Fields

        private Uri _darkSource;
        private Uri _darkHighlightSource;
        private Uri _lightSource;

        #endregion

        #region Properties

        public Uri DarkSource 
        { 
            get => _darkSource; 
            set
            {
                _darkSource = value;
            }
        }

        public Uri DarkHighlightSource
        {
            get => _darkHighlightSource;
            set
            {
                _darkHighlightSource = value;
            }
        }

        public Uri LightSource
        {
            get => _lightSource;
            set
            {
                _lightSource = value;
            }
        }

        #endregion

        #region Methods

        public void UpdateTheme(Theme theme)
        {
            switch (theme) 
            {
                case Theme.Dark:
                    Source = _darkSource;
                    break;
                case Theme.DarkHighlight:
                    Source = _darkHighlightSource;
                    break;
                case Theme.Light:
                    Source = _lightSource;
                    break;
                default:
                    Source = _darkSource;
                    break;
            }
        }

        #endregion
    }
}
