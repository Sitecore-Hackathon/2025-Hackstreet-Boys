using Sitecore.CH.Cli.Core.Rendering;

namespace HackStreetCLIExtensions.Views
{
    public class ErrorView(string message) : IRenderable
    {
        public void Render()
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine($"{DateTime.Now} :: {message}");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
