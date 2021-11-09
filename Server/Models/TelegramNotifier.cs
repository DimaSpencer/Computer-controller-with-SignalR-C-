using Server.Abstractions;
using System.Threading.Tasks;
using Telegram.Bot;

namespace Server.Models
{
    public class TelegramNotifier : INotifier
    {
        private readonly INotifier _notifier;

        private const string TELEGRAM_TOKEN = "1864364281:AAEkTuAWO6U5X7vuzrBfn5sZSUKFKSieayk";

        private readonly int _chatId;
        private readonly TelegramBotClient _telegramBot = new TelegramBotClient(TELEGRAM_TOKEN);

        public TelegramNotifier(int chatId, INotifier notifier = null)
        {
            _chatId = chatId;
            _notifier = notifier;
        }

        public async Task NotifyAsync(string message)
        {
            await _telegramBot.SendTextMessageAsync(758726820, message);
            await _notifier?.NotifyAsync(message);
        }
    }
}
