using System;
using System.IO;
using FluentAssertions;
using Ionic.Zip;
using Malin.Client;
using Malin.Host;
using NUnit.Framework;

namespace Malin.Tests
{
    [TestFixture]
    public class MalinTests
    {
        [Test]
        public void ClientAndServer()
        {
            using (var consoleOut = new StringWriter())
            {
                // Arrange
                const string authorizationToken = "8676F020-29ED-45DE-920B-9DD268470174";
                const string hostUrl = "http://localhost:3131";
                const string uploadFile = "Test.zip";

                File.WriteAllText("Deploy.ps1", @"echo ""Hello, World!"" >HelloWorld.txt
Write-Host ""Logoutput from deployment script.""");

                using (var zip = new ZipFile(uploadFile)) 
                {
                    zip.AddFile("Deploy.ps1");
                    zip.Save();
                }

                Console.SetOut(consoleOut);

                // Act
                int clientExitCode;
                using (var host = MalinHost.Start("/port:" + (new Uri(hostUrl)).Port))
                {
                    host.UnpackDestination = "unpack";
                    host.AuthorizationToken = authorizationToken;

                    clientExitCode = Program.Main(new[] {"deploy", hostUrl, uploadFile, authorizationToken});
                }

                // Assert
                clientExitCode.Should().Be(0);
                File.Exists(@"unpack\Test\HelloWorld.txt").Should().BeTrue();
                consoleOut.ToString().Should().Contain("Logoutput from deployment script.");
            }
        }

        [Test]
        public void ClientAndServer_DeployScriptThrowsError_ClientReturnsWithErrorExitCode()
        {
            using (var consoleOut = new StringWriter())
            {
                // Arrange
                const string authorizationToken = "8676F020-29ED-45DE-920B-9DD268470174";
                const string hostUrl = "http://localhost:3131";
                const string uploadFile = "Test.zip";

                File.WriteAllText("Deploy.ps1", @"throw ""Three cheers for the fallen ones!""");

                using (var zip = new ZipFile(uploadFile))
                {
                    zip.AddFile("Deploy.ps1");
                    zip.Save();
                }

                Console.SetOut(consoleOut);

                // Act
                int clientExitCode;
                using (var host = MalinHost.Start("/port:" + (new Uri(hostUrl)).Port))
                {
                    host.UnpackDestination = "unpack";
                    host.AuthorizationToken = authorizationToken;

                    clientExitCode = Program.Main(new[] { "deploy", hostUrl, uploadFile, authorizationToken });
                }

                // Assert
                clientExitCode.Should().NotBe(0);
                consoleOut.ToString().Should().Contain("Three cheers for the fallen ones!");
            }
        }

        [Test]
        [Category("Slow")]
        public void ClientAndServer_SlowDeploy()
        {
            using (var consoleOut = new StringWriter())
            {
                // Arrange
                const string authorizationToken = "8676F020-29ED-45DE-920B-9DD268470174";
                const string hostUrl = "http://localhost:3131";
                const string uploadFile = "Test.zip";

                File.WriteAllText("Deploy.ps1", @"[System.Threading.Thread]::Sleep(2 * 60 * 1000)");

                using (var zip = new ZipFile(uploadFile))
                {
                    zip.AddFile("Deploy.ps1");
                    zip.Save();
                }

                Console.SetOut(consoleOut);

                // Act
                int clientExitCode;
                using (var host = MalinHost.Start("/port:" + (new Uri(hostUrl)).Port))
                {
                    host.UnpackDestination = "unpack";
                    host.AuthorizationToken = authorizationToken;

                    clientExitCode = Program.Main(new[] { "deploy", hostUrl, uploadFile, authorizationToken });
                }

                // Assert
                clientExitCode.Should().Be(0);
            }
        }

        [SetUp]
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
    }
}
