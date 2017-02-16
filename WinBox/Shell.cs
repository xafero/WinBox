using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using log4net;

namespace WinBox
{
	public static class Shell
	{
		private static readonly ILog log = LogManager.GetLogger("shell");
		
		public static void DownloadPacker(string root, NameValueCollection config)
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
		
		public static void ExecutePacker(string packerExe, string pack, NameValueCollection config)
		{
			var builder = config["builder"];
			var procInfo = new ProcessStartInfo
			{
				UseShellExecute = false,
				FileName = packerExe,
				Arguments = string.Format("build -force -only {0} {1}", builder, pack)
			};
			log.InfoFormat("Starting => {0} {1}", procInfo.FileName, procInfo.Arguments);
			using (var proc = Process.Start(procInfo))
			{
				proc.WaitForExit();
			}
		}
	}
}