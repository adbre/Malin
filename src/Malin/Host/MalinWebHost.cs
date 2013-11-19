using System;
using System.Linq;
using Microsoft.Owin.Hosting;

namespace Malin.Host
{
    public class MalinWebHost : IDisposable
    {
        private IDisposable _webApp;

        private MalinWebHost()
        {
        }

        public static MalinWebHost Start(string[] args)
        {
            var port = 3030;
            if (args != null)
            {
                var portArgument = args.FirstOrDefault(arg => arg.StartsWith("/port:"));
                if (!string.IsNullOrWhiteSpace(portArgument))
                {
                    port = int.Parse(portArgument.Substring("/port:".Length));
                }
            }

            var result = new MalinWebHost
                {
                    _webApp = WebApp.Start<MalinHostStartup>("http://+:" + port)
                };

            return result;
        }

        public void Dispose()
        {
            if (_webApp != null) _webApp.Dispose(); ;
        }

        public string UnpackDestination
        {
            get { return Properties.Settings.Default.UnpackZipFileDestination; }
            set { Properties.Settings.Default.UnpackZipFileDestination = value; }
        }

        public string AuthorizationToken
        {
            get { return Properties.Settings.Default.AuthorizationToken; }
            set { Properties.Settings.Default.AuthorizationToken = value; }
        }

        public int ArtifactsToKeep
        {
            get { return Properties.Settings.Default.ArtifactsToKeep; }
            set { Properties.Settings.Default.ArtifactsToKeep = value; }
        }
    }
}