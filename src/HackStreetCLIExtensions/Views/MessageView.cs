using Sitecore.CH.Cli.Core.Rendering;

namespace HackStreetCLIExtensions.Views
{
    public class MessageView(string message) : IRenderable
    {
        public void Render()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"{DateTime.Now} :: {message}");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
