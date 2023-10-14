using DeltaSql.Enums;
using DeltaSql.Properties;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading.Tasks;
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
        private ConnectionStatus connectionStatus = ConnectionStatus.NotConnected;
        private ICommand connectionStringCommand;
        private string connectionString = string.Empty;
        private string database = string.Empty;
        private ICommand disconnectCommand;
        private AcceptanceState infoEntryAcceptanceState;
        private string infoEntryWarningError = string.Empty;
        private AcceptanceState manualEntryAcceptanceState;
        private string manualEntryWarningError = string.Empty;
        private bool manualMode;
        private string password = string.Empty;
        private ObservableCollection<string> previousConnections = new ObservableCollection<string>();
        private int previousConnectionSelectedIndex = -1;
        private int selectedAuthMode;
        private string server = string.Empty;
        private SqlConnectionStringBuilder sqlConnectionStringBuilder;
        private string sqlVersion;
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

        public ConnectionStatus ConnectionStatus 
        { 
            get => connectionStatus; 
            set
            {
                connectionStatus = value;
                OnPropertyChanged();
            }
        }

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

        public ICommand DisconnectCommand => disconnectCommand ?? (disconnectCommand = new RelayCommand(Disconnect, CanDisconnect));

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

        public int PreviousConnectionSelectedIndex 
        { 
            get => previousConnectionSelectedIndex; 
            set
            {
                previousConnectionSelectedIndex = value;
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

        public SqlConnection SqlConnection { get; private set; }

        public string SqlVersion
        {
            get => sqlVersion;
            set
            {
                sqlVersion = value;
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

        #region Events

        public event EventHandler Connected;
        public event EventHandler<CancelEventArgs> Connecting;
        public event EventHandler Disconnected;

        #endregion

        #region Methods

        private bool CanConnect()
        {
            // previous connections take precedence
            if (PreviousConnectionSelectedIndex > -1 &&
                (ConnectionStatus == ConnectionStatus.NotConnected || ConnectionStatus == ConnectionStatus.DatabaseConnectionRequired || ConnectionStatus == ConnectionStatus.ServerConnectionRequired)) return true;

            if (ManualMode)
            {
                return !string.IsNullOrWhiteSpace(ConnectionString) && 
                    (ConnectionStatus == ConnectionStatus.NotConnected || ConnectionStatus == ConnectionStatus.DatabaseConnectionRequired || ConnectionStatus == ConnectionStatus.ServerConnectionRequired) &&
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

                return ConnectionStatus == ConnectionStatus.NotConnected || ConnectionStatus == ConnectionStatus.DatabaseConnectionRequired || ConnectionStatus == ConnectionStatus.ServerConnectionRequired;
            }
        }

        private bool CanDisconnect()
        {
            return SqlConnection != null;
        }

        private void Clear()
        {
            // if the control is connected when the user clicks clear then don't do anything, make them click disconnect (then they can click clear)
            if (ConnectionStatus == ConnectionStatus.DatabaseConnected || ConnectionStatus == ConnectionStatus.ServerConnected)
                return;

            ConnectionString = string.Empty;
            Database = string.Empty;
            InfoEntryAcceptanceState = AcceptanceState.None;
            InfoEntryWarningError = string.Empty;
            ManualEntryAcceptanceState = AcceptanceState.None;
            ManualEntryWarningError = string.Empty;
            Password = string.Empty;
            PreviousConnectionSelectedIndex = -1;
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

            CancelEventArgs cancelEventArgs = new CancelEventArgs();

            Connecting?.Invoke(this, cancelEventArgs);

            if (cancelEventArgs.Cancel) return;

            if (!string.IsNullOrWhiteSpace(sqlConnectionStringBuilder.InitialCatalog))
            {
                ServiceLocator.Instance.LoggingService.Debug(
                    string.Format(Translations.AttemptingDatabaseConnection, 
                        sqlConnectionStringBuilder.InitialCatalog));
            }
            else
            {
                ServiceLocator.Instance.LoggingService.Debug(
                    string.Format(Translations.AttemptingServerConnection, 
                        sqlConnectionStringBuilder.DataSource));
            }

            ConnectionAttempt();
        }

        private void ConnectionAttempt()
        {
            string cnt = string.IsNullOrWhiteSpace(sqlConnectionStringBuilder.InitialCatalog) ? sqlConnectionStringBuilder.DataSource : sqlConnectionStringBuilder.InitialCatalog;

            ServiceLocator.Instance.MainWindowViewModel.ProgressViewModel.ProgressTitle = Translations.ConnectingMessageTitle;
            ServiceLocator.Instance.MainWindowViewModel.ProgressViewModel.ProgressIsIndeterminate = true;

            if (string.IsNullOrWhiteSpace(sqlConnectionStringBuilder.InitialCatalog))
                ServiceLocator.Instance.MainWindowViewModel.ProgressViewModel.ProgressMessage =
                    string.Format(Translations.AttemptingServerConnection, cnt);
            else
                ServiceLocator.Instance.MainWindowViewModel.ProgressViewModel.ProgressMessage =
                    string.Format(Translations.AttemptingDatabaseConnection, cnt);

            ServiceLocator.Instance.MainWindowViewModel.ProgressViewModel.ProgressDialogVisbility = Visibility.Visible;

            Task.Run(() =>
            {
                try
                {
                    SqlConnection = new SqlConnection(sqlConnectionStringBuilder.ConnectionString);

                    SqlConnection.Open();

                    // get the version of SQL we are connected to
                    SqlCommand command = new SqlCommand("SELECT SERVERPROPERTY('productversion')", SqlConnection);

                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                        SqlVersion = reader.GetString(0);

                    reader.Close();
                    reader.Dispose();

                    command.Dispose();

                    SqlConnection.Close();
                }
                catch (Exception ex)
                {
                    SqlConnection = null;

                    string exMes = ex.Message.EnsurePeriodAtEnd();

                    ServiceLocator.Instance.LoggingService.Error(string.Format(Translations.ConnectionError, cnt, exMes));

                    if (ManualMode)
                    {
                        ManualEntryAcceptanceState = AcceptanceState.Error;
                        ManualEntryWarningError = exMes;
                    }
                    else
                    {
                        InfoEntryAcceptanceState = AcceptanceState.Error;
                        InfoEntryWarningError = exMes;
                    }
                }
            }).ContinueWith(task =>
            {
                ServiceLocator.Instance.MainWindowViewModel.ProgressViewModel.ProgressMessage = string.Empty;
                ServiceLocator.Instance.MainWindowViewModel.ProgressViewModel.ProgressTitle = string.Empty;
                ServiceLocator.Instance.MainWindowViewModel.ProgressViewModel.ProgressIsIndeterminate = false;
                ServiceLocator.Instance.MainWindowViewModel.ProgressViewModel.ProgressDialogVisbility = Visibility.Collapsed;

                if (SqlConnection == null)
                {
                    string cnt = string.IsNullOrWhiteSpace(sqlConnectionStringBuilder.InitialCatalog) ? sqlConnectionStringBuilder.DataSource : sqlConnectionStringBuilder.InitialCatalog;

                    ServiceLocator.Instance.LoggingService.Warning(string.Format(Translations.ConnectionError2, cnt));

                    return;
                }

                if (task.Exception == null || !task.IsFaulted)
                {
                    if (string.IsNullOrWhiteSpace(sqlConnectionStringBuilder.InitialCatalog))
                    {
                        ConnectionStatus = ConnectionStatus.ServerConnected;

                        ServiceLocator.Instance.LoggingService.Info(string.Format(Translations.ConnectedToServer, sqlConnectionStringBuilder.DataSource));
                    }
                    else
                    {
                        ConnectionStatus = ConnectionStatus.DatabaseConnected;

                        ServiceLocator.Instance.LoggingService.Info(string.Format(Translations.ConnectedToDatabase, sqlConnectionStringBuilder.InitialCatalog));
                    }

                    Connected?.Invoke(this, EventArgs.Empty);
                }
            });
        }

        public void ConnectIfAble()
        {
            if (CanConnect()) Connect();
        }

        private void ConnectionStringsCom()
        {
            Process.Start(new ProcessStartInfo("https://www.connectionstrings.com/sql-server/") { UseShellExecute = true });
        }

        private void Disconnect()
        {
            // we don't hold connections to the database open, so just clean up the object
            SqlConnection.Dispose();
            SqlConnection = null;

            ConnectionStatus = ConnectionStatus.NotConnected;
            PreviousConnectionSelectedIndex = -1;

            if (ConnectionStatus == ConnectionStatus.DatabaseConnected)
                ServiceLocator.Instance.LoggingService.Info(string.Format(Translations.DisconnectingDatabase, sqlConnectionStringBuilder.InitialCatalog));
            else if (ConnectionStatus == ConnectionStatus.ServerConnected)
                ServiceLocator.Instance.LoggingService.Info(string.Format(Translations.DisconnectingServer, sqlConnectionStringBuilder.DataSource));

            Disconnected?.Invoke(this, EventArgs.Empty);
        }

        private bool VerifyConnectionString()
        {
            if (!ManualMode)
            {
                string temp = $"Server={Server};";

                if (!string.IsNullOrWhiteSpace(Database))
                    temp += $"Initial Catalog={Database};";

                if (SelectedAuthMode == 0) // Windows auth
                    temp += "Integrated Security=True;";
                else if (SelectedAuthMode == 1) // SQL auth
                    temp += $"User Id={Username};Password={Password};";

                ConnectionString = temp;
            }

            // regardless of which top portion was selected, if the user selected a previous connection string then that will take precedence
            if (PreviousConnectionSelectedIndex > -1)
                ConnectionString = ServiceLocator.Instance.Cryptographer.Decrypt(Settings.Default.PreviousConnections[PreviousConnectionSelectedIndex]);

            sqlConnectionStringBuilder = new SqlConnectionStringBuilder(ConnectionString);

            if (ConnectionStatus == ConnectionStatus.NotConnected)
            {
                if (string.IsNullOrWhiteSpace(sqlConnectionStringBuilder.DataSource))
                {
                    WriteStringToAppropriateSqlInputStatus(Translations.MissingServerName, AcceptanceState.Error);

                    return false;
                }

                if (!sqlConnectionStringBuilder.IntegratedSecurity) // means SQL auth
                {
                    if (string.IsNullOrWhiteSpace(sqlConnectionStringBuilder.UserID))
                    {
                        WriteStringToAppropriateSqlInputStatus(Translations.MissingUsername, AcceptanceState.Error);

                        return false;
                    }

                    if (string.IsNullOrWhiteSpace(sqlConnectionStringBuilder.Password))
                    {
                        WriteStringToAppropriateSqlInputStatus(Translations.MissingPassword, AcceptanceState.Error);

                        return false;
                    }
                }
            }
            else if (ConnectionStatus == ConnectionStatus.DatabaseConnectionRequired)
            {
                if (string.IsNullOrWhiteSpace(sqlConnectionStringBuilder.InitialCatalog))
                {
                    WriteStringToAppropriateSqlInputStatus(Translations.DatabaseRequired, AcceptanceState.Error);

                    return false;
                }
            }
            else if (ConnectionStatus == ConnectionStatus.ServerConnectionRequired)
            {
                if (!string.IsNullOrWhiteSpace(sqlConnectionStringBuilder.InitialCatalog))
                {
                    WriteStringToAppropriateSqlInputStatus(Translations.ServerRequired, AcceptanceState.Error);

                    return false;
                }
            }

            // make sure to update the connection string to the one constructed from the builder
            ConnectionString = sqlConnectionStringBuilder.ConnectionString;

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
                ManualEntryWarningError = string.Format(Translations.ConnectionStringContainsParameter, temp.Substring(0, temp.Length - 1));
                ManualEntryAcceptanceState = AcceptanceState.Error;
            }
            else ConnectionString += $"{parameter};";
        }

        public void WriteStringToAppropriateSqlInputStatus(string message, AcceptanceState acceptanceState)
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
