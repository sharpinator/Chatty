using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharpinator.Chatty.ViewModels
{
    public interface IAlertMessageService
    {
        Task ShowAsync(string message, string title, IEnumerable<DialogCommand> command = null);
    }
    public class DialogCommand
    {
        public object Id { get; set; }
        public string Label { get; set; }
        public Action Invoked { get; set; }

    }
}
