using System;
using System.Threading.Tasks;
using VirusClient.Models;

namespace VirsulClientCMD
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Virus virus = new Virus(new Uri(@"https://localhost:44316/hub"));
            await virus.StartAsync();
            while (true)
            {
            }
        }
    }
}
