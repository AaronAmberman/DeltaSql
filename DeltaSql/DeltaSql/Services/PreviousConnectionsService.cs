using DeltaSql.Properties;
using DeltaSql.ViewModels;
using System;
using System.Collections.Generic;

namespace DeltaSql.Services
{
    internal class PreviousConnectionsService : IPreviousConnectionsService
    {
        #region Methods

        public bool AddConnectionString(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString)) return false;

            bool match = false;

            foreach (string pc in Settings.Default.PreviousConnections)
            {
                string decrypted = string.Empty;

                try
                {
                    decrypted = ServiceLocator.Instance.Cryptographer.Decrypt(pc);
                }
                catch (Exception ex)
                {
                    ServiceLocator.Instance.LoggingService.Logger.Error($"Error: could not decrypt the previous connection string {pc}.{Environment.NewLine}{ex.Message}");
                }

                if (string.IsNullOrWhiteSpace(decrypted))
                    continue;

                if (decrypted.Equals(connectionString)) match = true;
            }

            if (!match)
            {
                Settings.Default.PreviousConnections.Add(ServiceLocator.Instance.Cryptographer.Encrypt(connectionString));
                Settings.Default.Save();

                return true;
            }

            return false;
        }

        public void AddNewConnectionString(string connectionString)
        {
            if (ServiceLocator.Instance.PreviousConnectionsService.AddConnectionString(connectionString))
            {
                string viewFriendlyPasswordConStr = ServiceLocator.Instance.PreviousConnectionsService.HidePasswordInConnectionString(connectionString);

                ServiceLocator.Instance.MainWindowViewModel.SqlInputViewModelLeft.PreviousConnections.Add(viewFriendlyPasswordConStr);
                ServiceLocator.Instance.MainWindowViewModel.SqlInputViewModelRight.PreviousConnections.Add(viewFriendlyPasswordConStr);
            }
        }

        public string HidePasswordInConnectionString(string connectionString) 
        {
            string temp = connectionString;

            if (temp.Contains("Password=", StringComparison.OrdinalIgnoreCase))
            {
                int start = temp.IndexOf("Password=") + 9;
                int end = temp.IndexOf(';', start);

                // if we cannot find the ';' to end the password we will assume it is the last parameter
                string sub = string.Empty;

                if (end == -1) sub = temp.Substring(start);
                else sub = temp.Substring(start, end - start);

                temp = temp.Replace(sub, "***");
            }

            return temp;
        }

        public void Initialize()
        {
            if (Settings.Default.PreviousConnections == null)
                Settings.Default.PreviousConnections = new System.Collections.Specialized.StringCollection();

            if (Settings.Default.PreviousConnections.Count > 0)
            {
                List<string> prevCons = ServiceLocator.Instance.PreviousConnectionsService.ReadInPreviousConnectionStrings();

                if (prevCons.Count > 0)
                {
                    ServiceLocator.Instance.MainWindowViewModel.SqlInputViewModelLeft.PreviousConnections.AddRange(prevCons);
                    ServiceLocator.Instance.MainWindowViewModel.SqlInputViewModelRight.PreviousConnections.AddRange(prevCons);
                }
            }
        }

        public List<string> ReadInPreviousConnectionStrings()
        {
            List<string> list = new List<string>();

            foreach (string str in Settings.Default.PreviousConnections)
            {
                string decrypted = ServiceLocator.Instance.Cryptographer.Decrypt(str);                

                string friendly = HidePasswordInConnectionString(decrypted);

                list.Add(friendly);
            }

            return list;
        }

        #endregion
    }
}
