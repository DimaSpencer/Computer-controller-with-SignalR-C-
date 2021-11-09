using MimeKit;
using System.Threading.Tasks;

namespace VirusControllerLibrary.Models
{
    public class EmailNotifier : INotifier
    {
        private readonly INotifier _notifier;

        private readonly string _targetAddress;
        private readonly MimeMessage _emailSender = new MimeMessage();

        public EmailNotifier(string targetEmail, string subject, INotifier notifier = null)
        {
            //if (string.IsNullOrEmpty(message))
            //    throw new ArgumentNullException("Email address is null");

            //_targetAddress = targetEmail;
            //_emailSender.From.Add(new MailboxAddress("EmailNotifier", "DimaSpencer55@gmail.com"));
            //_emailSender.To.Add(new MailboxAddress("", email));
            //_emailSender.Subject = subject;
            //_emailSender.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            //{
            //    Text = message
            //};

            _notifier = notifier;
        }

        public async Task NotifyAsync(string message)
        {
            //using (SmtpClient client = new SmtpClient())
            //{
            //    await client.ConnectAsync("smtp.yandex.ru", 25, false);
            //    await client.AuthenticateAsync("login@yandex.ru", "password");
            //    await client.SendAsync(_targetAddress);

            //    await client.DisconnectAsync(true);
            //}
            await _notifier?.NotifyAsync(message);
        }
    }
}
