using DeltaSql.Enums;
using System;
using System.Data.SqlClient;
using System.Windows.Input;
using WPF.Translations;

namespace DeltaSql.ViewModels
{
    internal class ConnectionViewModel : ViewModelBase
    {
        #region Fields

        private string connectionName;
        private ConnectionStatus connectionStatus = ConnectionStatus.NotConnected;
        private ICommand disconnectCommand;
        private SqlConnection sqlConnection;
        private string serverVersion;
        private Translation translations;

        #endregion

        #region Events

        public event EventHandler Disconnected;

        #endregion

        #region Properties

        public string ConnectionName 
        { 
            get => connectionName;
            set
            {
                connectionName = value;
                OnPropertyChanged();
            }
        }

        public ConnectionStatus ConnectionStatus
        {
            get => connectionStatus;
            set
            {
                connectionStatus = value;
                OnPropertyChanged();
            }
        }

        public ICommand DisconnectCommand => disconnectCommand ?? (disconnectCommand = new RelayCommand(Disconnect));

        public SqlConnection SqlConnection 
        { 
            get => sqlConnection; 
            set
            {
                sqlConnection = value;
                OnPropertyChanged();
            }
        }

        public string ServerVersion
        {
            get => serverVersion;
            set
            {
                serverVersion = value;
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

        #endregion

        #region Methods

        private void Disconnect()
        {
            Disconnected?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}
