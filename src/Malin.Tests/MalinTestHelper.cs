using System;
using System.IO;
using Ionic.Zip;

namespace Malin.Tests
{
    public static class MalinTestHelper
    {
        public static void CreateZipFileWithPsScript(string fileName, string script, string scriptFileName = null)
        {
            if (fileName == null) throw new ArgumentNullException("fileName");
            if (script == null) throw new ArgumentNullException("script");
            scriptFileName = scriptFileName ?? "Deploy.ps1";

            File.WriteAllText(scriptFileName, script);

            using (var zip = new ZipFile(fileName))
            {
                zip.AddFile(scriptFileName);
                zip.Save();
            }
        }
    }
}