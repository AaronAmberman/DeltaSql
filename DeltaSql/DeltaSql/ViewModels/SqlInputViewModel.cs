using DeltaSql.Enums;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using WPF.Translations;

namespace DeltaSql.ViewModels
{
    internal class SqlInputViewModel : ViewModelBase
    {
        #region Fields

        private ICommand clearCommand;
        private ICommand connectCommand;
        private ICommand connectionStringCommand;
        private string connectionString = string.Empty;
        private string database = string.Empty;
        private AcceptanceState infoEntryAcceptanceState;
        private string infoEntryWarningError = string.Empty;
        private AcceptanceState manualEntryAcceptanceState;
        private string manualEntryWarningError = string.Empty;
        private bool manualMode;
        private string password = string.Empty;
        private ObservableCollection<string> previousConnections = new ObservableCollection<string>();
        private int selectedAuthMode;
        private string server = string.Empty;
        private Translation translations;
        private string username = string.Empty;
        private Visibility visibility = Visibility.Visible;
        private Visibility visibilityInfoEntry = Visibility.Visible;
        private Visibility visibilityManualEntry = Visibility.Collapsed;
        private ICommand wordsServerCommand;
        private ICommand wordsInitialCatalogCommand;
        private ICommand wordsUsernameCommand;
        private ICommand wordsPasswordCommand;
        private ICommand wordsIntegratedSecurityCommand;        

        #endregion

        #region Properties

        public ICommand ClearCommand => clearCommand ?? (clearCommand = new RelayCommand(Clear));

        public ICommand ConnectCommand => connectCommand ?? (connectCommand = new RelayCommand(Connect, CanConnect));

        public ICommand ConnectionStringCommand => connectionStringCommand ?? (connectionStringCommand = new RelayCommand(ConnectionStringsCom));

        public string ConnectionString
        {
            get => connectionString;
            set
            {
                connectionString = value;
                OnPropertyChanged();
            }
        }

        public string Database
        {
            get => database;
            set
            {
                database = value;
                OnPropertyChanged();
            }
        }

        public AcceptanceState InfoEntryAcceptanceState
        {
            get => infoEntryAcceptanceState;
            set
            {
                infoEntryAcceptanceState = value;
                OnPropertyChanged();
            }
        }

        public string InfoEntryWarningError
        {
            get => infoEntryWarningError;
            set
            {
                infoEntryWarningError = value;
                OnPropertyChanged();
            }
        }

        public AcceptanceState ManualEntryAcceptanceState
        {
            get => manualEntryAcceptanceState;
            set
            {
                manualEntryAcceptanceState = value;
                OnPropertyChanged();
            }
        }

        public string ManualEntryWarningError
        {
            get => manualEntryWarningError;
            set
            {
                manualEntryWarningError = value;
                OnPropertyChanged();
            }
        }

        public bool ManualMode
        {
            get => manualMode;
            set
            {
                manualMode = value;

                if (value)
                {
                    VisibilityInfoEntry = Visibility.Collapsed;
                    VisibilityManualEntry = Visibility.Visible;
                }
                else
                {
                    VisibilityInfoEntry = Visibility.Visible;
                    VisibilityManualEntry = Visibility.Collapsed;
                }

                OnPropertyChanged();
            }
        }

        public string Password
        {
            get => password;
            set
            {
                password = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> PreviousConnections 
        {
            get => previousConnections;
            set
            {
                previousConnections = value;
                OnPropertyChanged();
            }
        }

        public int SelectedAuthMode
        {
            get => selectedAuthMode;
            set
            {
                selectedAuthMode = value;
                OnPropertyChanged();
            }
        }

        public string Server
        {
            get => server;
            set
            {
                server = value;
                OnPropertyChanged();
            }
        }

        public dynamic Translations 
        {
            get => translations; 
            set
            {
                translations = value;
                OnPropertyChanged();
            }
        }

        public string Username
        {
            get => username;
            set
            {
                username = value;
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

        public Visibility VisibilityInfoEntry
        {
            get => visibilityInfoEntry;
            set
            {
                visibilityInfoEntry = value;
                OnPropertyChanged();
            }
        }

        public Visibility VisibilityManualEntry
        {
            get => visibilityManualEntry;
            set
            {
                visibilityManualEntry = value;
                OnPropertyChanged();
            }
        }

        public ICommand WordsServerCommand => wordsServerCommand ?? (wordsServerCommand = new RelayCommand(WordsServer));

        public ICommand WordsInitialCatalogCommand => wordsInitialCatalogCommand ?? (wordsInitialCatalogCommand = new RelayCommand(WordsInitialCatalog));

        public ICommand WordsUsernameCommand => wordsUsernameCommand ?? (wordsUsernameCommand = new RelayCommand(WordsUsername));

        public ICommand WordsPasswordCommand => wordsPasswordCommand ?? (wordsPasswordCommand = new RelayCommand(WordsPassword));

        public ICommand WordsIntegratedSecurityCommand => wordsIntegratedSecurityCommand ?? (wordsIntegratedSecurityCommand = new RelayCommand(WordsIntegratedSecurity));

        #endregion

        #region Methods

        private bool CanConnect()
        {
            if (ManualMode)
            {
                return !string.IsNullOrWhiteSpace(ConnectionString) &&
                    ((ConnectionString.Contains("Server=") && ConnectionString.Contains("User Id=") && ConnectionString.Contains("Password=")) ||
                    (ConnectionString.Contains("Server=") && ConnectionString.Contains("Integrated Security=")));
            }
            else
            {
                if (string.IsNullOrWhiteSpace(Server))
                    return false;

                if (SelectedAuthMode == 1) // SQL auth
                {
                    if (string.IsNullOrWhiteSpace(Username)) return false;
                    if (string.IsNullOrWhiteSpace(Password)) return false;
                }

                return true;
            }
        }

        private void Clear()
        {
            ConnectionString = string.Empty;
            Database = string.Empty;
            InfoEntryAcceptanceState = AcceptanceState.None;
            InfoEntryWarningError = string.Empty;
            ManualEntryAcceptanceState = AcceptanceState.None;
            ManualEntryWarningError = string.Empty;
            Password = string.Empty;
            Server = string.Empty;
            Username = string.Empty;
        }

        private void ClearInfoStatus()
        {
            InfoEntryAcceptanceState = AcceptanceState.None;
            InfoEntryWarningError = string.Empty;
        }

        private void ClearManualStatus()
        {
            ManualEntryAcceptanceState = AcceptanceState.None;
            ManualEntryWarningError = string.Empty;
        }

        private void Connect()
        {
            ClearInfoStatus();
            ClearManualStatus();

            if (!VerifyConnectionString()) return;

            SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder(ConnectionString);

            if (!string.IsNullOrWhiteSpace(sqlConnectionStringBuilder.InitialCatalog))
            {
                ServiceLocator.Instance.LoggingService.Debug(
                    string.Format(ServiceLocator.Instance.MainWindowViewModel.Translations.AttemptingDatabaseConnection, 
                        sqlConnectionStringBuilder.InitialCatalog));
            }
            else
            {
                ServiceLocator.Instance.LoggingService.Debug(
                    string.Format(ServiceLocator.Instance.MainWindowViewModel.Translations.AttemptingServerConnection, 
                        sqlConnectionStringBuilder.DataSource));
            }
        }

        private void ConnectionStringsCom()
        {
            Process.Start(new ProcessStartInfo("https://www.connectionstrings.com/sql-server/") { UseShellExecute = true });
        }

        private bool VerifyConnectionString()
        {
            if (!ManualMode)
            {
                string temp = $"Server={Server};";

                if (!string.IsNullOrWhiteSpace(Database))
                    temp += $"Initial Catalog={Database};";

                if (SelectedAuthMode == 0) // Windows auth
                {
                    temp += "Integrated Security=True;";
                }
                else if (SelectedAuthMode == 1) // SQL auth
                {
                    temp += $"User Id={Username};Password={Password};";
                }

                ConnectionString = temp;
            }

            SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder(ConnectionString);

            if (string.IsNullOrWhiteSpace(sqlConnectionStringBuilder.DataSource))
            {
                WriteStringToAppropriateSqlInputStatus(ServiceLocator.Instance.MainWindowViewModel.Translations.MissingServerName, AcceptanceState.Error);

                return false;
            }

            if (!sqlConnectionStringBuilder.IntegratedSecurity) // means SQL auth
            {
                if (string.IsNullOrWhiteSpace(sqlConnectionStringBuilder.UserID))
                {
                    WriteStringToAppropriateSqlInputStatus(ServiceLocator.Instance.MainWindowViewModel.Translations.MissingUsername, AcceptanceState.Error);

                    return false;
                }

                if (string.IsNullOrWhiteSpace(sqlConnectionStringBuilder.Password))
                {
                    WriteStringToAppropriateSqlInputStatus(ServiceLocator.Instance.MainWindowViewModel.Translations.MissingPassword, AcceptanceState.Error);

                    return false;
                }
            }

            return true;
        }

        private void WordsInitialCatalog()
        {
            WordsX("Initial Catalog="); // does this need to be translated?
        }

        private void WordsIntegratedSecurity()
        {
            WordsX("Integrated Security=True"); // does this need to be translated?
        }

        private void WordsPassword()
        {
            WordsX("Password="); // does this need to be translated?
        }

        private void WordsServer()
        {
            WordsX("Server="); // does this need to be translated?
        }

        private void WordsUsername()
        {
            WordsX("User Id="); // does this need to be translated?
        }

        private void WordsX(string parameter)
        {
            ClearManualStatus();

            string temp = parameter.Substring(0, parameter.IndexOf("=") + 1);

            if (ConnectionString.Contains(temp))
            {
                ManualEntryWarningError = string.Format(ServiceLocator.Instance.MainWindowViewModel.Translations.ConnectionStringContainsParameter, temp.Substring(0, temp.Length - 1));
                ManualEntryAcceptanceState = AcceptanceState.Error;
            }
            else ConnectionString += $"{parameter};";
        }

        private void WriteStringToAppropriateSqlInputStatus(string message, AcceptanceState acceptanceState)
        {
            if (ManualMode)
            {
                ManualEntryAcceptanceState = acceptanceState;
                ManualEntryWarningError = message;
            }
            else
            {
                InfoEntryAcceptanceState = acceptanceState;
                InfoEntryWarningError = message;
            }
        }

        #endregion
    }
}
