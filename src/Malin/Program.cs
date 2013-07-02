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
            var action = args.FirstOrDefault();
            if (action == "deploy")
            {
                return MalinClient.Run(args.Skip(1).ToArray());
            }
            if (action == "host" || action == "/host" || !Environment.UserInteractive)
            {
                return MalinHost.Run(args);
            }

            Console.Error.WriteLine("Unkown action '{0}'. Supported actions are 'deploy' or 'host'.", action);
            return 1;
        }
    }
}
