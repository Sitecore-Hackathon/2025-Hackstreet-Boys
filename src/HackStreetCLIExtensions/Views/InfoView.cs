using Sitecore.CH.Cli.Core.Rendering;

namespace HackStreetCLIExtensions.Views
{
    public class InfoView(string message) : IRenderable
    {
        public void Render()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"{DateTime.Now} :: {message}");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
