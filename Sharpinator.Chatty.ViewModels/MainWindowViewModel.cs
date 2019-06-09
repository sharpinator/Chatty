using Microsoft.AspNetCore.SignalR.Client;
using Prism.Commands;
using Prism.Windows.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Sharpinator.Chatty.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public struct MessageWrapper
        {
            public string UserName { get; set; }
            public string Message { get; set; }
        }
        
        public ICommand SendMessage { get; private set; }
        public ICommand Connect { get; private set; }
        public ICommand Disconnect { get; private set; }
        public ObservableCollection<MessageWrapper> Messages { get; private set; } = new ObservableCollection<MessageWrapper>();
        
        private bool isConnected;
        public bool IsConnected
        {
            get { return isConnected; }
            private set
            {
                if(SetProperty(ref isConnected, value))
                    RaisePropertyChanged(nameof(IsDisconnected));
            }
        }
        public bool IsDisconnected { get { return !IsConnected; } }
        private string userName = "My Name";
        public string UserName
        {
            get { return userName; }
            set
            {
                this.SetProperty(ref userName, value);
            }
        }
        private string message = "";
        public string Message
        {
            get { return message; }
            set
            {
                SetProperty(ref message, value);
            }
        }
        protected HubConnection Connection { get; private set; }
        protected IDispatcher Dispatcher { get; private set; }
        protected IDisposable MessageReceived { get; private set; }
        protected IAlertMessageService AlertService { get; private set; }
        public MainPageViewModel(IHubConnectionBuilder builder, IDispatcher dispatcher, IAlertMessageService alertService)
        {
            SendMessage = new DelegateCommand<string>(DoSendMessage);
            Connect = new DelegateCommand(DoConnect);
            Disconnect = new DelegateCommand(DoDisconnect);
            Connection = builder.Build();
            Dispatcher = dispatcher;
            AlertService = alertService;
        }

        private async void DoSendMessage(string message)
        {
            try
            {
                await Connection.InvokeAsync("newMessage", UserName, message);
                Message = "";
            }
            catch (Exception ex)
            {
                await AlertService.ShowAsync(ex.Message, "Couldn't Send Message");
            }
        }
        private async void DoConnect()
        {
            try
            {
                MessageReceived = Connection.On<string, string>("messageReceived", AddMessage);
                await Connection.StartAsync();
                IsConnected = true;
            }
            catch(Exception ex)
            {
                await AlertService.ShowAsync(ex.Message, "Couldn't Connect");
            }
            
        }
        private async void DoDisconnect()
        {
            try
            {
                IsConnected = false;
                MessageReceived.Dispose();
                MessageReceived = null;
                await Connection.StopAsync();
            }
            catch(Exception ex)
            {
                await AlertService.ShowAsync(ex.Message, "Couldn't Disconnect");
            }
        }
        private async void AddMessage(string username, string message)
        {
            await Dispatcher.RunAsync(() => Messages.Add(new MessageWrapper() { UserName = username, Message = message }));
        }
    }
}
