using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Linq;
using Ionic.Zip;

namespace WinBox.Boot
{
    public static class Payload
    {
        public static void HandlePayload(string file, IEnumerable<KeyValuePair<string, string>> vars)
        {
            var spec = Environment.SpecialFolder.LocalApplicationData;
            var folder = Environment.GetFolderPath(spec);
            var path = Path.Combine(folder, "WinBox");
            using (var zip = new ZipFile(file))
            {
                var opt = ExtractExistingFileAction.OverwriteSilently;
                zip.ExtractAll(path, opt);
                var sopt = SearchOption.TopDirectoryOnly;
                var files = Directory.GetFiles(path, "*.bat", sopt)
                    .Concat(Directory.GetFiles(path, "*.cmd", sopt))
                    .Concat(Directory.GetFiles(path, "*.exe", sopt));
                var exeFile = files.First();
                var args = Environment.CommandLine.Trim();
                var procInfo = new ProcessStartInfo
                {
                    WorkingDirectory = path,
                    FileName = exeFile,
                    Arguments = args,
                    CreateNoWindow = false,
                    LoadUserProfile = false,
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Normal
                };
                Array.ForEach(vars.ToArray(), e => procInfo.EnvironmentVariables[e.Key] = e.Value);
                using (var proc = Process.Start(procInfo))
                {
                    proc.WaitForExit(5);
                }
            }
        }
    }
}