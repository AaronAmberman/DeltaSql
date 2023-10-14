using DeltaSql.Enums;
using SimpleLogger;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using WPF.InternalDialogs;
using WPF.Translations;

namespace DeltaSql.ViewModels
{
    internal class MainWindowViewModel : ViewModelBase
    {
        #region Fields

        private CancellationTokenSource cancellationTokenSource;
        private ICommand showSettingsCommand;
        private Translation translations;
        private string version;

        #endregion

        #region Properties

        public ConnectionViewModel ConnectionViewModelLeft { get; set; }

        public ConnectionViewModel ConnectionViewModelRight { get; set; }

        public Dispatcher Dispatcher { get; set; }

        public RichTextBox RichTextBox { get; set; }

        public MessageBoxViewModel MessageBoxViewModel { get; set; }

        public ProgressViewModel ProgressViewModel { get; set; }

        public SettingsViewModel SettingsViewModel { get; set; }

        public ICommand ShowSettingsCommand => showSettingsCommand ??= new RelayCommand(ShowSettings);

        public SqlInputViewModel SqlInputViewModelLeft { get; set; }

        public SqlInputViewModel SqlInputViewModelRight { get; set; }

        public dynamic Translations 
        { 
            get => translations; 
            set
            {
                translations = value;

                ConnectionViewModelLeft.Translations = value;
                ConnectionViewModelRight.Translations = value;
                SqlInputViewModelLeft.Translations = value;
                SqlInputViewModelRight.Translations = value;

                OnPropertyChanged();
                
            }
        }

        public string Version 
        { 
            get => version; 
            set
            {
                version = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Methods

        public void BeginInvoke(Action action)
        {
            Dispatcher.BeginInvoke(action);
        }

        public void SqlInputViewModel_Connected(object sender, EventArgs e)
        {
            string conStr = string.Empty;

            if (sender == SqlInputViewModelLeft)
            {
                // right side is already connected, don't do anything
                if (!(SqlInputViewModelRight.ConnectionStatus == ConnectionStatus.DatabaseConnected || SqlInputViewModelRight.ConnectionStatus == ConnectionStatus.ServerConnected))
                {
                    if (SqlInputViewModelLeft.ConnectionStatus == ConnectionStatus.DatabaseConnected)
                        SqlInputViewModelRight.ConnectionStatus = ConnectionStatus.DatabaseConnectionRequired;
                    else if (SqlInputViewModelLeft.ConnectionStatus == ConnectionStatus.ServerConnected)
                        SqlInputViewModelRight.ConnectionStatus = ConnectionStatus.ServerConnectionRequired;
                }

                conStr = SqlInputViewModelLeft.ConnectionString;
            }
            else if (sender == SqlInputViewModelRight)
            {
                // left side is already connected, don't do anything
                if (!(SqlInputViewModelLeft.ConnectionStatus == ConnectionStatus.DatabaseConnected || SqlInputViewModelLeft.ConnectionStatus == ConnectionStatus.ServerConnected))
                {
                    if (SqlInputViewModelRight.ConnectionStatus == ConnectionStatus.DatabaseConnected)
                        SqlInputViewModelLeft.ConnectionStatus = ConnectionStatus.DatabaseConnectionRequired;
                    else if (SqlInputViewModelRight.ConnectionStatus == ConnectionStatus.ServerConnected)
                        SqlInputViewModelLeft.ConnectionStatus = ConnectionStatus.ServerConnectionRequired;
                }

                conStr = SqlInputViewModelRight.ConnectionString;
            }

            // handle previous connection setting
            if (!string.IsNullOrWhiteSpace(conStr))
            {
                Invoke(() => { ServiceLocator.Instance.PreviousConnectionsService.AddNewConnectionString(conStr); });
            }

            // close if we have both connections
            if (SqlInputViewModelLeft.SqlConnection != null && SqlInputViewModelRight.SqlConnection != null)
            {
                SqlInputViewModelLeft.Visibility = Visibility.Collapsed;
                SqlInputViewModelRight.Visibility = Visibility.Collapsed;

                ConnectionViewModelLeft.ConnectionStatus = SqlInputViewModelLeft.ConnectionStatus;
                ConnectionViewModelLeft.SqlConnection = SqlInputViewModelLeft.SqlConnection;
                ConnectionViewModelLeft.ServerVersion = string.Format(Translations.SqlServerVersion, SqlInputViewModelLeft.SqlVersion);

                ConnectionViewModelRight.ConnectionStatus = SqlInputViewModelRight.ConnectionStatus;
                ConnectionViewModelRight.SqlConnection = SqlInputViewModelRight.SqlConnection;
                ConnectionViewModelRight.ServerVersion = string.Format(Translations.SqlServerVersion, SqlInputViewModelRight.SqlVersion);

                SqlConnectionStringBuilder left = new SqlConnectionStringBuilder(SqlInputViewModelLeft.ConnectionString);
                SqlConnectionStringBuilder right = new SqlConnectionStringBuilder(SqlInputViewModelRight.ConnectionString);

                string leftName = string.IsNullOrWhiteSpace(left.InitialCatalog) ? left.DataSource : left.InitialCatalog;
                string rightName = string.IsNullOrWhiteSpace(right.InitialCatalog) ? right.DataSource : right.InitialCatalog;

                ConnectionViewModelLeft.ConnectionName = leftName;
                ConnectionViewModelRight.ConnectionName = rightName;
            }
        }

        public void SqlInputViewModel_Connecting(object sender, CancelEventArgs e)
        {
            // make sure they both have connection strings before we attempt to compare them
            if (string.IsNullOrWhiteSpace(SqlInputViewModelLeft.ConnectionString) && !string.IsNullOrWhiteSpace(SqlInputViewModelRight.ConnectionString)) return;
            if (!string.IsNullOrWhiteSpace(SqlInputViewModelLeft.ConnectionString) && string.IsNullOrWhiteSpace(SqlInputViewModelRight.ConnectionString)) return;

            // make sure the left and right side are not trying to connect to the same data source!!!
            SqlConnectionStringBuilder left = new SqlConnectionStringBuilder(SqlInputViewModelLeft.ConnectionString);
            SqlConnectionStringBuilder right = new SqlConnectionStringBuilder(SqlInputViewModelRight.ConnectionString);

            string message = string.Empty;

            // we require matching connection types (database & database or server and server)
            if (string.IsNullOrEmpty(left.InitialCatalog) && !string.IsNullOrEmpty(right.InitialCatalog))
                message = Translations.ConnectionTypeDoesNotMatch;

            // only continue to process if we don't have an existing issue
            if (string.IsNullOrEmpty(message))
            {
                if (!string.IsNullOrEmpty(left.InitialCatalog) && string.IsNullOrEmpty(right.InitialCatalog))
                    message = Translations.ConnectionTypeDoesNotMatch;
            }

            // only continue to process if we don't have an existing issue
            if (string.IsNullOrEmpty(message))
            {
                if (string.IsNullOrEmpty(left.InitialCatalog))
                {
                    // compare servers only
                    if (left.DataSource.Equals(right.DataSource, StringComparison.OrdinalIgnoreCase))
                        message = Translations.SameDataSource;
                }
                else
                {
                    // compare servers and databases
                    if (left.DataSource.Equals(right.DataSource, StringComparison.OrdinalIgnoreCase) && 
                        left.InitialCatalog.Equals(right.InitialCatalog, StringComparison.OrdinalIgnoreCase))
                        message = Translations.SameDataSource;
                }
            }

            // if we have a message, we have an error of some kind...cancel
            if (!string.IsNullOrEmpty(message))
            {
                SqlInputViewModel vm = sender as SqlInputViewModel;
                vm?.WriteStringToAppropriateSqlInputStatus(message, AcceptanceState.Error);

                e.Cancel = true;
            }
        }

        public void SqlInputViewModel_Disconnected(object sender, EventArgs e)
        {
            if (sender == SqlInputViewModelLeft) SqlInputViewModelRight.ConnectionStatus = ConnectionStatus.NotConnected;
            else if (sender == SqlInputViewModelRight) SqlInputViewModelLeft.ConnectionStatus = ConnectionStatus.NotConnected;
        }

        public void Invoke(Action action)
        {
            Dispatcher.Invoke(action);
        }

        public void LoggingService_LogEntry(object sender, (LogLevel LogLevel, string Message) e)
        {
            Invoke(() => 
            {
                if (RichTextBox == null) return;

                if (RichTextBox.Document == null)
                    RichTextBox.Document = new FlowDocument();

                // if blank lines are desired in the output feed then use a space
                List<Block> emptyBlocks = RichTextBox.Document.Blocks.Where(b => string.IsNullOrEmpty(new TextRange(b.ContentStart, b.ContentEnd).Text)).ToList();

                foreach (Block b in emptyBlocks) RichTextBox.Document.Blocks.Remove(b);

                if (string.IsNullOrWhiteSpace(e.Message)) return;

                Paragraph p = new Paragraph
                {
                    Margin = new Thickness(0),
                    Padding = new Thickness(0)
                };

                switch (e.LogLevel)
                {
                    case LogLevel.Debug: p.Inlines.Add(new Italic(new Run(e.Message) { Foreground = Brushes.CornflowerBlue })); break;
                    case LogLevel.Error: p.Inlines.Add(new Bold(new Run(e.Message) { Foreground = Brushes.Red })); break;
                    case LogLevel.Fatal: p.Inlines.Add(new Bold(new Italic(new Run(e.Message) { Foreground = Brushes.DarkRed }))); break;
                    case LogLevel.Trace: p.Inlines.Add(new Italic(new Run(e.Message) { Foreground = Brushes.SandyBrown })); break;
                    case LogLevel.Warning: p.Inlines.Add(new Bold(new Run(e.Message) { Foreground = Brushes.Orange })); break;
                    case LogLevel.Info:
                        p.Inlines.Add(new Run(e.Message)
                        {
                            Tag = "Info",
                            Style = Application.Current.Resources["LoggingInfoRunStyle"] as Style
                        }); 
                        break;
                }

                RichTextBox.Document.Blocks.Add(p);
            });
        }

        private void SetMessageBoxState(string message, string title, bool isModal, MessageBoxButton button, MessageBoxInternalDialogImage image, Visibility visibility)
        {
            MessageBoxViewModel.MessageBoxMessage = message;
            MessageBoxViewModel.MessageBoxTitle = title;
            MessageBoxViewModel.MessageBoxIsModal = isModal;
            MessageBoxViewModel.MessageBoxButton = button;
            MessageBoxViewModel.MessageBoxImage = image;
            MessageBoxViewModel.MessageBoxVisibility = visibility;
        }

        public void ShowMessageBox(string message)
        {
            SetMessageBoxState(message, string.Empty, true, MessageBoxButton.OK, MessageBoxInternalDialogImage.Information, Visibility.Visible);
        }

        public void ShowMessageBox(string message, string title)
        {
            SetMessageBoxState(message, title, true, MessageBoxButton.OK, MessageBoxInternalDialogImage.Information, Visibility.Visible);
        }

        public void ShowMessageBox(string message, string title, MessageBoxButton button)
        {
            SetMessageBoxState(message, title, true, button, MessageBoxInternalDialogImage.Information, Visibility.Visible);
        }

        public void ShowMessageBox(string message, string title, MessageBoxInternalDialogImage image)
        {
            SetMessageBoxState(message, title, true, MessageBoxButton.OK, image, Visibility.Visible);
        }

        public void ShowMessageBox(string message, string title, MessageBoxButton button, MessageBoxInternalDialogImage image)
        {
            SetMessageBoxState(message, title, true, button, image, Visibility.Visible);
        }

        public MessageBoxResult ShowQuestionBox(string question, string title)
        {
            MessageBoxViewModel.MessageBoxMessage = question;
            MessageBoxViewModel.MessageBoxTitle = title;
            MessageBoxViewModel.MessageBoxIsModal = true;
            MessageBoxViewModel.MessageBoxButton = MessageBoxButton.YesNo;
            MessageBoxViewModel.MessageBoxImage = MessageBoxInternalDialogImage.Help;
            MessageBoxViewModel.MessageBoxVisibility = Visibility.Visible; // this will block because of is modal

            return MessageBoxViewModel.MessageBoxResult;
        }

        private void ShowSettings()
        {
            SettingsViewModel.Visibility = Visibility.Visible;
        }

        private void ThreadedSetMessageBoxState(string message, string title, bool isModal, MessageBoxButton button, MessageBoxInternalDialogImage image, Visibility visibility)
        {
            Invoke(() => { SetMessageBoxState(message, title, isModal, button, image, visibility); });
        }

        public void ThreadedShowMessageBox(string message)
        {
            ThreadedSetMessageBoxState(message, string.Empty, true, MessageBoxButton.OK, MessageBoxInternalDialogImage.Information, Visibility.Visible);
        }

        public void ThreadedShowMessageBox(string message, string title)
        {
            ThreadedSetMessageBoxState(message, title, true, MessageBoxButton.OK, MessageBoxInternalDialogImage.Information, Visibility.Visible);
        }

        public void ThreadedShowMessageBox(string message, string title, MessageBoxButton button)
        {
            ThreadedSetMessageBoxState(message, title, true, button, MessageBoxInternalDialogImage.Information, Visibility.Visible);
        }

        public void ThreadedShowMessageBox(string message, string title, MessageBoxInternalDialogImage image)
        {
            ThreadedSetMessageBoxState(message, title, true, MessageBoxButton.OK, image, Visibility.Visible);
        }

        public void ThreadedShowMessageBox(string message, string title, MessageBoxButton button, MessageBoxInternalDialogImage image)
        {
            ThreadedSetMessageBoxState(message, title, true, button, image, Visibility.Visible);
        }

        public MessageBoxResult ThreadedShowQuestionBox(string question, string title)
        {
            return Dispatcher.Invoke(() => { return ShowQuestionBox(question, title); });
        }

        #endregion
    }
}
