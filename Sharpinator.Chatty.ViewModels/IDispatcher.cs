using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace Sharpinator.Chatty.ViewModels
{
    public interface IDispatcher
    {
        Task RunAsync(DispatchedHandler action, CoreDispatcherPriority priority = CoreDispatcherPriority.Normal);
    }

    public class CoreDispatcherProxy : IDispatcher
    {
        protected CoreDispatcher Dispatcher { get; private set; }
        public CoreDispatcherProxy()
        {
            Dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;
        }
        public async Task RunAsync(DispatchedHandler action, CoreDispatcherPriority priority = CoreDispatcherPriority.Normal)
        {
            await Dispatcher.RunAsync(priority, action);
        }
    }
}
