using System;
using System.Linq;
using Malin.Client;
using Malin.Host;

namespace Malin
{
    public class Program
    {
        public static int Main(string[] args)
        {
            return args.Any(s => s == "/host") || !Environment.UserInteractive
                       ? MalinHost.Run(args)
                       : MalinClient.Run(args);
        }
    }
}
