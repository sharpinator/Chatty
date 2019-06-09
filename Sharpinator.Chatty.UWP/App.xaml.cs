using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Practices.Unity;
using Prism.Mvvm;
using Prism.Unity.Windows;
using Sharpinator.Chatty.ViewModels;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Sharpinator.Chatty.UWP.Services;

namespace Sharpinator.Chatty.UWP
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : PrismUnityApplication
    {
        public App()
        {

            InitializeComponent();

            
        }
        protected override UIElement CreateShell(Frame rootFrame)
        {

            var shell = Container.Resolve<Shell>();

            shell.SetContentFrame(rootFrame);

            return shell;

        }



        /// <summary>

        /// Logic of app initialization.

        /// This is the best place to register the services in Unity container.

        /// </summary>

        /// <param name="args"></param>

        /// <returns></returns>

        protected override void ConfigureContainer()
        {
            
            Container.RegisterInstance<IHubConnectionBuilder>(new HubConnectionBuilder().WithUrl("http://localhost:63113/ChatHub"));
            Container.RegisterInstance<IDispatcher>(new CoreDispatcherProxy());
            Container.RegisterInstance<IAlertMessageService>(new AlertMessageService());
            base.ConfigureContainer();
        }
        protected override void ConfigureViewModelLocator()
        {
            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver((viewType) =>
            {
                var assembly = ((dynamic) typeof(MainPageViewModel)).Assembly.FullName;
                var typeName = $"Sharpinator.Chatty.ViewModels.{viewType.Name}ViewModel,{assembly}";
                return Type.GetType(typeName); 
            });
            base.ConfigureViewModelLocator();
        }
        protected override Task OnLaunchApplicationAsync(LaunchActivatedEventArgs args)
        {
            NavigationService.Navigate("Main", null);
            return Task.CompletedTask;
        }
    }
}