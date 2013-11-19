using System;
using System.IO;
using System.Linq;
using System.Text;
using Nancy;
using Nancy.Responses;

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
                    DeleteOldArtifacts(logWriter);

                    var unpackDestination = Path.Combine(UnpackDestination, Path.GetFileNameWithoutExtension(httpFile.Name));
                    new DeployZipFileCommand(logWriter, unpackDestination).Execute(httpFile.Value);
                }
                catch (Exception error)
                {
                    logWriter.WriteLine(error);
                    return new TextResponse(HttpStatusCode.BadRequest, logWriter.ToString());
                }

                return new TextResponse(HttpStatusCode.OK, logWriter.ToString());
            }
        }

        private void DeleteOldArtifacts(TextWriter logWriter)
        {
            var artifactsToKeep = Properties.Settings.Default.ArtifactsToKeep - 1;
            if (artifactsToKeep < 1) return;

            var parentDirectory = new DirectoryInfo(UnpackDestination);
            if (!parentDirectory.Exists) return;

            logWriter.WriteLine("Removing old artifacts (keep {0})", artifactsToKeep);

            var directories = parentDirectory
                .GetDirectories()
                .OrderByDescending(d => d.CreationTime)
                .Skip(artifactsToKeep);

            foreach (var directory in directories)
            {
                logWriter.Write("Removing {0}... ", directory.FullName);
                directory.Delete(true);
                logWriter.WriteLine("OK");
            }

            logWriter.WriteLine("Done removing older artifacts.");
            logWriter.WriteLine();
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