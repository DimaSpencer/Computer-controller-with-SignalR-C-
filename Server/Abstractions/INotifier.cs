using System.Threading.Tasks;

namespace Server.Abstractions
{
    public interface INotifier
    {
        Task NotifyAsync(string message);
    }
}
