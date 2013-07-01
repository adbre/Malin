using System;
using System.Diagnostics;
using System.IO;
using Ionic.Zip;

namespace Malin.Host
{
    public class DeployZipFileCommand
    {
        public DeployZipFileCommand(TextWriter logWriter, string unpackDestination)
        {
            LogWriter = logWriter;
            UnpackDestination = unpackDestination;
        }

        public TextWriter LogWriter { get; private set; }
        public string UnpackDestination { get; set; }

        public void Execute(Stream stream)
        {
            Execute(stream, UnpackDestination);
        }

        private void Execute(Stream stream, string path)
        {
            CreateDirectoryIfNotExists(path);
            RemoveChildItems(path);
            UnpackZipFile(stream, path);
            FindAndExecuteDeployScript(path);
        }

        private void CreateDirectoryIfNotExists(string path)
        {
            if (!Directory.Exists(path))
            {
                LogWriter.WriteLine("Creating directory {0} ...", path);
                Directory.CreateDirectory(path);
            }
            else
            {
                LogWriter.WriteLine("Directory {0} already exists...", path);
            }
        }

        private void RemoveChildItems(string path)
        {
            LogWriter.WriteLine("Removing child items in directory {0} ...", path);
            var childItems = Directory.GetFileSystemEntries(path);
            foreach (var childItem in childItems)
            {
                if (Directory.Exists(childItem))
                    Directory.Delete(childItem, true);
                else
                    File.Delete(childItem);
            }
        }

        private void UnpackZipFile(Stream stream, string path)
        {
            LogWriter.WriteLine("Unpacking zip file into {0} ...", path);
            using (var zipFile = ZipFile.Read(stream))
            {
                foreach (var entry in zipFile)
                {
                    LogWriter.WriteLine("Unpacking {0}", entry.FileName);
                    entry.Extract(path, ExtractExistingFileAction.OverwriteSilently);
                }
            }
        }

        private void FindAndExecuteDeployScript(string path)
        {
            var deployScript = Path.Combine(path, "Deploy.ps1");
            if (!File.Exists(deployScript))
            {
                throw new FileNotFoundException("Could not find a deploy script", deployScript);
            }

            var scriptContents = string.Format(@"@powershell -ExecutionPolicy unrestricted -Command .\{0}", Path.GetFileName(deployScript));
            var tempBatFile = Path.Combine(Path.GetTempPath(), string.Format("{0}.bat", Guid.NewGuid().ToString()));
            File.WriteAllText(tempBatFile, scriptContents);

            LogWriter.WriteLine("Executing script: {0}", scriptContents);

            LogWriter.WriteLine("{1}> {0}", tempBatFile, path);
            StartProcessAndSetWorkingDirectory(tempBatFile, path);
        }

        private void StartProcessAndSetWorkingDirectory(string path, string workingDirectory)
        {
            var startInfo = new ProcessStartInfo(path)
                {
                    WorkingDirectory = workingDirectory
                };

            StartProcessAndForwardOutputToLog(startInfo);
        }

        private void StartProcessAndForwardOutputToLog(ProcessStartInfo startInfo)
        {
            var waitTimeout = TimeSpan.FromMinutes(20);

            using (var process = new Process())
            {
                process.StartInfo = SetRequiredStartInfoOptions(startInfo);
                process.OutputDataReceived += (sender, args) => LogWriter.WriteLine(args.Data);
                process.ErrorDataReceived += (sender, args) => LogWriter.WriteLine(args.Data);

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                process.WaitForExit((int)waitTimeout.TotalMilliseconds);
                if (!process.HasExited)
                {
                    process.Kill();
                    throw new InvalidOperationException(string.Format("{0} may have hung, have been waiting for {1} seconds.", startInfo.FileName, waitTimeout.TotalSeconds));
                }

                LogWriter.WriteLine("{0} returned with exit code {1}", startInfo.FileName, process.ExitCode);

                if (process.ExitCode != 0)
                    throw new InvalidOperationException(string.Format("{0} exited with error", startInfo.FileName));
            }
        }

        private ProcessStartInfo SetRequiredStartInfoOptions(ProcessStartInfo startInfo)
        {
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            return startInfo;
        }
    }
}