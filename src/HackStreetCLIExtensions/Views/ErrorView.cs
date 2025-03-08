using Sitecore.CH.Cli.Core.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackStreetCLIExtensions.Views
{
    public class ErrorView(string message) : IRenderable
    {
        public void Render()
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine($"{DateTime.Now} :: {message}");
        }
    }
}
