using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Security.Cryptography;
using log4net;

namespace WinBox
{
	public static class Shell
	{
		private static readonly ILog log = LogManager.GetLogger("shell");
		
		public static void DownloadPacker(string root, IDictionary<string, string> config)
		{
			var packerUrl = config["packer"];
			log.InfoFormat("Downloading from => {0}", packerUrl);
			SecurityProtocolType security;
			if (Enum.TryParse(config["security"], true, out security))
				ServicePointManager.SecurityProtocol = security;
			using (var client = new WebClient())
			{
				var zipPath = Path.Combine(root, "packer.zip");
				client.DownloadFile(packerUrl, zipPath);
				log.InfoFormat("Downloaded '{0}'!", zipPath);
				ZipFile.ExtractToDirectory(zipPath, root);
			}
		}
		
		public static void ExecutePacker(string workDir, string packerExe, string pack, IDictionary<string, string> config)
		{
			var builder = config["builder"];
			var procInfo = new ProcessStartInfo
			{
				UseShellExecute = false,
				WorkingDirectory = workDir,
				FileName = packerExe,
				Arguments = string.Format("build -force -only {0} {1}", builder, pack)
			};
			log.InfoFormat("Starting => {0} {1}", procInfo.FileName, procInfo.Arguments);
			using (var proc = Process.Start(procInfo))
			{
				proc.WaitForExit(5);
				var payload = Path.Combine(config["payload"]);
				log.InfoFormat("Payload root => {0}", payload);
				BootHoster.HostDirectory(payload);
				proc.Kill();
			}
		}
		
		public static string GetOrCreateHash(string fileName)
		{
			var hashFile = fileName + ".sha1";
			if (File.Exists(hashFile))
				return File.ReadAllText(hashFile);
			using (var fileStream = File.OpenRead(fileName))
			{
				using (var hashAlgo = new SHA1Managed())
				{
					var hashBytes = hashAlgo.ComputeHash(fileStream);
					var hashStr = BitConverter.ToString(hashBytes);
					hashStr = hashStr.ToLower().Replace("-", string.Empty);
					File.WriteAllText(hashFile, hashStr);
					return hashStr;
				}
			}
		}

		public static string FindNewestFile(string root, string pattern)
		{
			var files = Directory.GetFiles(root, pattern, SearchOption.AllDirectories);
			var lastChange = DateTime.MinValue;
			var lastCreate = DateTime.MinValue;
			var lastestFile = string.Empty;
			foreach (var file in files)
			{
				var changed = File.GetLastWriteTimeUtc(file);
				var created = File.GetCreationTimeUtc(file);
				if (changed >= lastChange || created >= lastCreate)
				{
					lastChange = changed;
					lastCreate = created;
					lastestFile = file;
				}
			}
			return string.IsNullOrWhiteSpace(lastestFile) ? null : lastestFile;
		}
	}
}