using System;
using System.ServiceProcess;

namespace Malin.Host
{
    public static class MalinHost
    {
        public static int Run(params string[] args)
        {
            return Environment.UserInteractive
                       ? RunInteractive(args)
                       : RunAsService(args);
        }

        private static int RunInteractive(params string[] args)
        {
            MalinLicense.WriteDisclaimer();

            using (Start(args))
            {
                Console.WriteLine("Press enter to exit");
                Console.ReadLine();
                return 0;
            }
        }

        private static int RunAsService(params string[] args)
        {
            using (var service = new MalinServiceHost(() => Start(args)))
            {
                ServiceBase.Run(service);
                return 0;
            }
        }

        public static MalinWebHost Start(params string[] args)
        {
            return MalinWebHost.Start(args);
        }
    }
}