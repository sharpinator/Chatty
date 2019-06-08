using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Hubs;
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
        protected IHubProxy Proxy { get; private set; }
        protected IDisposable MessageReceived { get; private set; }
        protected IDispatcher Dispatcher { get; private set; }
        public MainPageViewModel(HubConnection connection, IDispatcher dispatcher)
        {
            SendMessage = new DelegateCommand<string>(DoSendMessage);
            Connect = new DelegateCommand(DoConnect);
            Disconnect = new DelegateCommand(DoDisconnect);
            Connection = connection;
            Dispatcher = dispatcher;
        }

        private async void DoSendMessage(string message)
        {
            await this.Proxy.Invoke("newMessage", UserName, message);
            await this.Dispatcher.RunAsync(() => Messages.Add(new MessageWrapper() { UserName = userName, Message = message }));
            Message = "";
        }
        private async void DoConnect()
        {
            var proxy = Connection.CreateHubProxy("ChatHub");
            MessageReceived = proxy.On<string, string>("messageReceived", AddMessage);
            Proxy = proxy;
            await Connection.Start();
            IsConnected = true;
            
        }
        private void DoDisconnect()
        {
            IsConnected = false;
            MessageReceived.Dispose();
            Connection.Stop();
        }
        private async void AddMessage(string username, string message)
        {
            await Dispatcher.RunAsync(() => Messages.Add(new MessageWrapper() { UserName = username, Message = message }));
        }
    }
}
