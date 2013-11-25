using System.IO;
using NUnit.Framework;

namespace Malin.Tests
{
    [TestFixture]
    public class ArtifactCleanUpTests : MalinTestBase
    {
        [Test]
        public void ShouldKeepSixArtifacts()
        {
            // Arrange
            MalinTestHelper.CreateZipFileWithPsScript("Test1.zip", "echo Hello, World!");
            MalinTestHelper.CreateZipFileWithPsScript("Test2.zip", "echo Hello, World!");
            MalinTestHelper.CreateZipFileWithPsScript("Test3.zip", "echo Hello, World!");
            MalinTestHelper.CreateZipFileWithPsScript("Test4.zip", "echo Hello, World!");
            MalinTestHelper.CreateZipFileWithPsScript("Test5.zip", "echo Hello, World!");
            MalinTestHelper.CreateZipFileWithPsScript("Test6.zip", "echo Hello, World!");
            MalinTestHelper.CreateZipFileWithPsScript("Test7.zip", "echo Hello, World!");

            Host.UnpackDestination = "unpack";
            Host.ArtifactsToKeep = 6;

            // Act
            InvokeDeploy("Test1.zip");
            InvokeDeploy("Test2.zip");
            InvokeDeploy("Test3.zip");
            InvokeDeploy("Test4.zip");
            InvokeDeploy("Test5.zip");
            InvokeDeploy("Test6.zip");
            InvokeDeploy("Test7.zip");

            // Assert
            Assert.That(Directory.Exists("unpack/Test7"), Is.True, "#1 Directory.Exists(unpack/Test7)");
            Assert.That(Directory.Exists("unpack/Test6"), Is.True, "#2 Directory.Exists(unpack/Test6)");
            Assert.That(Directory.Exists("unpack/Test5"), Is.True, "#3 Directory.Exists(unpack/Test5)");
            Assert.That(Directory.Exists("unpack/Test4"), Is.True, "#4 Directory.Exists(unpack/Test4)");
            Assert.That(Directory.Exists("unpack/Test3"), Is.True, "#5 Directory.Exists(unpack/Test3)");
            Assert.That(Directory.Exists("unpack/Test2"), Is.True, "#6 Directory.Exists(unpack/Test2)");
            Assert.That(Directory.Exists("unpack/Test1"), Is.False, "#7 Directory.Exists(unpack/Test1)");
        }

        [Test]
        public void ShouldKeepSixLogFiles()
        {
            // Arrange
            MalinTestHelper.CreateZipFileWithPsScript("Test1.zip", "echo Hello, World!");
            MalinTestHelper.CreateZipFileWithPsScript("Test2.zip", "echo Hello, World!");
            MalinTestHelper.CreateZipFileWithPsScript("Test3.zip", "echo Hello, World!");
            MalinTestHelper.CreateZipFileWithPsScript("Test4.zip", "echo Hello, World!");
            MalinTestHelper.CreateZipFileWithPsScript("Test5.zip", "echo Hello, World!");
            MalinTestHelper.CreateZipFileWithPsScript("Test6.zip", "echo Hello, World!");
            MalinTestHelper.CreateZipFileWithPsScript("Test7.zip", "echo Hello, World!");

            Host.UnpackDestination = "unpack";
            Host.ArtifactsToKeep = 6;

            // Act
            InvokeDeploy("Test1.zip");
            InvokeDeploy("Test2.zip");
            InvokeDeploy("Test3.zip");
            InvokeDeploy("Test4.zip");
            InvokeDeploy("Test5.zip");
            InvokeDeploy("Test6.zip");
            InvokeDeploy("Test7.zip");

            // Assert
            Assert.That(File.Exists("unpack/Test7.log"), Is.True, "#1 File.Exists(unpack/Test7.log)");
            Assert.That(File.Exists("unpack/Test6.log"), Is.True, "#2 File.Exists(unpack/Test6.log)");
            Assert.That(File.Exists("unpack/Test5.log"), Is.True, "#3 File.Exists(unpack/Test5.log)");
            Assert.That(File.Exists("unpack/Test4.log"), Is.True, "#4 File.Exists(unpack/Test4.log)");
            Assert.That(File.Exists("unpack/Test3.log"), Is.True, "#5 File.Exists(unpack/Test3.log)");
            Assert.That(File.Exists("unpack/Test2.log"), Is.True, "#6 File.Exists(unpack/Test2.log)");
            Assert.That(File.Exists("unpack/Test1.log"), Is.False, "#7 File.Exists(unpack/Test1.log)");
        }

        [SetUp]
        public void DeleteExistingFiles()
        {
            foreach (var zipFile in Directory.GetFiles(".", "*.zip"))
                File.Delete(zipFile);
        }
    }
}