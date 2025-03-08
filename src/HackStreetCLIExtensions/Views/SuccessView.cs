using Sitecore.CH.Cli.Core.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackStreetCLIExtensions.Views
{
    public class SuccessView(string message) : IRenderable
    {
        public void Render()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{DateTime.Now} :: {message}");
        }
    }
}
