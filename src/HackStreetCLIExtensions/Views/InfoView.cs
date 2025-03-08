using Sitecore.CH.Cli.Core.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackStreetCLIExtensions.Views
{
    public class InfoView(string message) : IRenderable
    {
        public void Render()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"{DateTime.Now} :: {message}");
        }
    }
}
