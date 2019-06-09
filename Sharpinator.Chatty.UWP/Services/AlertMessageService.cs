using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sharpinator.Chatty.ViewModels;
using Windows.UI.Popups;

namespace Sharpinator.Chatty.UWP.Services
{
    public class AlertMessageService : IAlertMessageService

    {

        private static bool _isShowing = false;



        public async Task ShowAsync(string message, string title, IEnumerable<DialogCommand> dialogCommands = null)

        {

            // Only show one dialog at a time.
            if (!_isShowing)
            { 
                var messageDialog = new MessageDialog(message, title);
                if (dialogCommands != null)

                {
                    var commands = dialogCommands.Select(c => new UICommand(c.Label, (command) => c.Invoked(), c.Id));
                    foreach (var command in commands)
                    {
                        messageDialog.Commands.Add(command);
                    }
                }
                _isShowing = true;

                try
                {
                    await messageDialog.ShowAsync();
                }
                catch { throw; }
                finally { _isShowing = false; }
            }

        }

    }
}
