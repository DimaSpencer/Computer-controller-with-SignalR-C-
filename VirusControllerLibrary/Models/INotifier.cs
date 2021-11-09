using MailKit.Net.Smtp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace VirusControllerLibrary.Models
{
    public interface INotifier
    {
        Task NotifyAsync(string message);
    }
}
