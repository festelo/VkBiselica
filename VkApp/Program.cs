using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VkNet;
using VkNet.Model;

namespace VkApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var app = new VkApp();
            app.AuthorizeAsync().Wait();
            app.Listen();
        }
    }
}
