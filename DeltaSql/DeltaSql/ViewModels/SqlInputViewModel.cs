using DeltaSql.Enums;
using System.Collections.ObjectModel;
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
            return !string.IsNullOrWhiteSpace(ConnectionString);
        }

        private void Clear()
        {
            ConnectionString = string.Empty;
            Database = string.Empty;
            InfoEntryWarningError = string.Empty;
            ManualEntryWarningError = string.Empty;
            Password = string.Empty;
            Server = string.Empty;
            Username = string.Empty;
        }

        private void ClearManualStatus()
        {
            ManualEntryAcceptanceState = AcceptanceState.None;
            ManualEntryWarningError = string.Empty;
        }

        private void Connect()
        {
            
        }

        private void ConnectionStringsCom()
        {
            Process.Start(new ProcessStartInfo("https://www.connectionstrings.com/sql-server/") { UseShellExecute = true });
        }

        private void WordsInitialCatalog()
        {
            WordsX("Initial Catalog="); // does this need to be translated?
        }

        private void WordsIntegratedSecurity()
        {
            WordsX("Integrated Security="); // does this need to be translated?
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

            if (ConnectionString.Contains(parameter))
            {
                ManualEntryWarningError = string.Format(ServiceLocator.Instance.MainWindowViewModel.Translations.ConnectionStringContainsParameter, parameter.Substring(0, parameter.Length - 1));
                ManualEntryAcceptanceState = AcceptanceState.Error;
            }
            else
                ConnectionString += $"{parameter};";
        }

        #endregion
    }
}
