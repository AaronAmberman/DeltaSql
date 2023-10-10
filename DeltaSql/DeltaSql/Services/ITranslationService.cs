using System.Collections.Generic;

namespace DeltaSql.Services
{
    internal interface ITranslationService
    {
        #region Properties

        IDictionary<string, string> Languages { get; }
        dynamic Translations { get; set; }

        #endregion

        #region Methods

        void SetThreadCultureAndTranslations(string culture, bool setSettings);

        #endregion        
    }
}
