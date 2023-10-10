using DeltaSql.Theming;

namespace DeltaSql.Services
{
    internal interface IThemingService
    {
        Theme Theme { get; set; }

        void InitializeTheme();
        void SetThemeForApp(int theme);
    }
}
