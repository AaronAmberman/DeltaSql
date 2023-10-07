using SimpleLogger;
using System;
using System.Collections.Generic;
using System.Linq;
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

        private ICommand showSettingsCommand;
        private Translation translations;
        private string version;

        #endregion

        #region Properties

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

        public void Invoke(Action action)
        {
            Dispatcher.Invoke(action);
        }

        public void LoggingService_LogEntry(object sender, (LogLevel, string) e)
        {
            if (RichTextBox == null) return;

            if (RichTextBox.Document == null)
            {
                RichTextBox.Document = new FlowDocument();
                RichTextBox.Document.Blocks.Clear();
            }

            // if blank lines are desired in the output feed then use a space
            List<Block> emptyBlocks = RichTextBox.Document.Blocks.Where(b => string.IsNullOrEmpty(new TextRange(b.ContentStart, b.ContentEnd).Text)).ToList();

            foreach (Block b in emptyBlocks) RichTextBox.Document.Blocks.Remove(b);

            if (string.IsNullOrWhiteSpace(e.Item2)) return;

            Paragraph p = new Paragraph
            {
                Margin = new Thickness(0),
                Padding = new Thickness(0)
            };

            switch (e.Item1)
            {
                case LogLevel.Debug: p.Inlines.Add(new Italic(new Run(e.Item2) { Foreground = Brushes.CornflowerBlue })); break;
                case LogLevel.Error: p.Inlines.Add(new Bold(new Run(e.Item2) { Foreground = Brushes.Red })); break;
                case LogLevel.Fatal: p.Inlines.Add(new Bold(new Italic(new Run(e.Item2) { Foreground = Brushes.DarkRed }))); break;
                case LogLevel.Info: p.Inlines.Add(new Run(e.Item2) { Foreground = Brushes.Black }); break;
                case LogLevel.Trace: p.Inlines.Add(new Italic(new Run(e.Item2) { Foreground = Brushes.SandyBrown })); break;
                case LogLevel.Warning: p.Inlines.Add(new Bold(new Run(e.Item2) { Foreground = Brushes.Orange })); break;
            }

            RichTextBox.Document.Blocks.Add(p);
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

        #endregion
    }
}
