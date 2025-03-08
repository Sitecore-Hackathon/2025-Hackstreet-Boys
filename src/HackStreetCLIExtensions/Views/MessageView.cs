using Sitecore.CH.Cli.Core.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackStreetCLIExtensions.Views
{
    public class MessageView(string message) : IRenderable
    {
        public void Render()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"{DateTime.Now} :: {message}");
        }
    }
}
