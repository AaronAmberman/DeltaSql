using System;
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
        private string connectionString;
        private string database;
        private bool manualMode;
        private string password;
        private ObservableCollection<string> previousConnections = new ObservableCollection<string>();
        private int selectedAuthMode;
        private string server;
        private Translation translations;
        private string username;
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
            return false;
        }

        private void Clear()
        {

        }

        private void Connect()
        {
            
        }

        private void ConnectionStringsCom()
        {
            Process.Start(new ProcessStartInfo("https://www.connectionstrings.com/sql-server/") { UseShellExecute = true });
        }

        private void WordsServer()
        {
            
        }

        private void WordsInitialCatalog()
        {
            
        }

        private void WordsUsername()
        {
            
        }

        private void WordsPassword()
        {
            
        }

        private void WordsIntegratedSecurity()
        {
            
        }

        #endregion
    }
}
