using System;
using System.IO;
using System.Net;
using System.Text;

namespace Malin.Client
{
    public class MalinClient
    {
        public static int Run(params string[] args)
        {
            if (args.Length < 2)
            {
                PrintUsage();
                return 1;
            }

            Uri uri;
            if (!Uri.TryCreate(args[0], UriKind.Absolute, out uri))
            {
                Console.Error.WriteLine("first argument must be a legal URI");
                return 2;
            }

            var program = new MalinClient()
                {
                    AuthorizationToken = args.Length > 2 ? args[2] : Environment.GetEnvironmentVariable("MALIN_AUTHTOKEN"),
                    RemoteUri = uri,
                    FileName = args[1],
                };

            if (program.RemoteUri.Scheme.ToLower() != Uri.UriSchemeHttp.ToLower())
            {
                Console.Error.WriteLine("URI Scheme must be HTTP");
                return 3;
            }

            if (string.IsNullOrWhiteSpace(program.AuthorizationToken))
            {
                Console.Error.WriteLine("Environment variable MALIN_AUTHTOKEN not set.");
                Console.Error.WriteLine("A authorization token is required in order to upload package.");
                return 4;
            }

            if (!File.Exists(program.FileName))
            {
                Console.Error.WriteLine("{0} does not exists", program.FileName);
                return 5;
            }

            try
            {
                program.Run();
                return 0;
            }
            catch (Exception error)
            {
                Console.Error.WriteLine(error);
                return 6;
            }
        }

        private static void PrintUsage()
        {
            Console.WriteLine("USAGE: malin <uri> <zipFile>");
            Console.WriteLine();
            Console.WriteLine("  uri        HTTP address to malin web host.");
            Console.WriteLine("  zipFile    Path to a .zip file containing the package to deploy.");
            Console.WriteLine();
        }

        public string AuthorizationToken { get; set; }
        public Uri RemoteUri { get; set; }
        public string FileName { get; set; }

        public Uri BuildUri(Uri baseUri)
        {
            var builder = new UriBuilder(baseUri);
            builder.Query = "?apiKey=" + AuthorizationToken;
            return builder.Uri;
        }

        public void Run()
        {
            using (var client = new WebClientUpload())
            {
                client.Timeout = TimeSpan.FromMinutes(10);

                var resonse = client.UploadFile(BuildUri(RemoteUri), FileName);
                Console.WriteLine(Encoding.UTF8.GetString(resonse));
            }
        }

        // Code copied from http://stackoverflow.com/questions/601861/set-timeout-for-webclient-downloadfile
        private class WebClientUpload : WebClient
        {
            public TimeSpan Timeout { get; set; }

            public WebClientUpload()
                : this(TimeSpan.FromMilliseconds(60000))
            {
            }

            public WebClientUpload(TimeSpan timeout)
            {
                Timeout = timeout;
            }

            protected override WebRequest GetWebRequest(Uri address)
            {
                var request = base.GetWebRequest(address);
                if (request != null)
                {
                    request.Timeout = (int)Timeout.TotalMilliseconds;
                }
                return request;
            }
        }
    }
}