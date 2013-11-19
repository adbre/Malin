using System;
using System.IO;
using Malin.Host;
using NUnit.Framework;

namespace Malin.Tests
{
    [TestFixture]
    public abstract class MalinTestBase
    {
        protected Uri HostUrl = new Uri("http://localhost:3131");
        protected string AuthorizationToken = "DBFC89A8-1696-49CD-BB92-33BCD9F01A28";
        protected string UnpackDirectory = "unpack";
        protected string UploadDirectory = "upload";

        protected MalinWebHost Host;

        [SetUp]
        public void StartMalinHost()
        {
            Host = MalinHost.Start("/port:" + HostUrl.Port);
            Host.UnpackDestination = UnpackDirectory;
            Host.AuthorizationToken = AuthorizationToken;

            DeleteFiles();
        }

        public void DeleteFiles()
        {
            Action<string> deleteIfExists = path =>
            {
                if (Directory.Exists(path)) Directory.Delete(path, true);
                if (File.Exists(path)) File.Delete(path);
            };

            deleteIfExists("Deploy.ps1");
            deleteIfExists("HelloWorld.txt");
            deleteIfExists("Test.zip");
            deleteIfExists("unpack");
        }

        [TearDown]
        public void StopMalinHost()
        {
            var host = Host;
            if (host != null) host.Dispose();
        }

        protected void InvokeDeploy(string fileName)
        {
            var exitCode = InvokeClient(new[] {"deploy", HostUrl.OriginalString, fileName, AuthorizationToken});
            Assert.That(exitCode, Is.EqualTo(0), "Exit Code");
        }

        protected int InvokeClient(params string[] args)
        {
            return Program.Main(args);
        }
    }
}