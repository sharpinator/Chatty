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
                SetProperty(ref isConnected, value);
            }
        }
        private string userName = Guid.NewGuid().ToString();
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
        public MainPageViewModel(IHubConnectionBuilder builder, IDispatcher dispatcher)
        {
            SendMessage = new DelegateCommand<string>(DoSendMessage);
            Connect = new DelegateCommand(DoConnect);
            Disconnect = new DelegateCommand(DoDisconnect);
            Connection = builder.Build();
            Dispatcher = dispatcher;
        }

        private async void DoSendMessage(string message)
        {
            await Connection.InvokeAsync("newMessage", UserName, message);
            Message = "";
        }
        private async void DoConnect()
        {
            Connection.On<string, string>("messageReceived", AddMessage);
            await Connection.StartAsync();
            IsConnected = true;
            
        }
        private async void DoDisconnect()
        {
            IsConnected = false;
            await Connection.StopAsync();
        }
        private async void AddMessage(string username, string message)
        {
            await Dispatcher.RunAsync(() => Messages.Add(new MessageWrapper() { UserName = username, Message = message }));
        }
    }
}
