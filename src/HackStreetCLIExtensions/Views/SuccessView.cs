using Sitecore.CH.Cli.Core.Rendering;

namespace HackStreetCLIExtensions.Views
{
    public class SuccessView(string message) : IRenderable
    {
        public void Render()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{DateTime.Now} :: {message}");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
