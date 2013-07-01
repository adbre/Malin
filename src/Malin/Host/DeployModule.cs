using System;
using System.IO;
using System.Linq;
using System.Text;
using Nancy;

namespace Malin.Host
{
    public class DeployModule : NancyModule
    {
        public string UnpackDestination
        {
            get { return Properties.Settings.Default.UnpackZipFileDestination; }
        }

        public string AuthorizationToken
        {
            get { return Properties.Settings.Default.AuthorizationToken; }
        }

        public DeployModule()
        {
            Get["/"] = x => Response.AsText(GetAbout());
            Post["/"] = x => UploadFile();
        }

        private string GetAbout()
        {
            var assemblyFileVersion = GetType().Assembly.Location;
            var fileVersionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(assemblyFileVersion);
            var versionInfo = new StringBuilder();
            versionInfo.AppendLine("Malin");
            versionInfo.AppendLine("Version: " + fileVersionInfo.ProductVersion);
            return versionInfo.ToString();
        }

        private object UploadFile()
        {
            if (!TryChallangeAuthorizationToken(Request.Query.apiKey))
                return HttpStatusCode.Unauthorized;

            var httpFile = Request.Files.FirstOrDefault();
            if (httpFile == null)
                return HttpStatusCode.BadRequest;

            using (var logWriter = new StringWriter())
            {
                try
                {
                    var unpackDestination = Path.Combine(UnpackDestination, Path.GetFileNameWithoutExtension(httpFile.Name));
                    new DeployZipFileCommand(logWriter, unpackDestination).Execute(httpFile.Value);
                }
                catch (Exception error)
                {
                    logWriter.WriteLine(error);
                }

                return logWriter.ToString();
            }

            return HttpStatusCode.OK;
        }

        private bool TryChallangeAuthorizationToken(string authorizationToken)
        {
            var expectedAuthorizationToken = AuthorizationToken;
            if (string.IsNullOrWhiteSpace(authorizationToken)) return false;
            if (string.IsNullOrWhiteSpace(expectedAuthorizationToken)) return false;
            return string.Equals(expectedAuthorizationToken, authorizationToken);
        }
    }
}