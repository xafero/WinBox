using System;
using System.IO;
using System.Diagnostics;
using System.Linq;
using Ionic.Zip;

namespace WinBox.Boot
{
	public static class Payload
	{
		public static void HandlePayload(string file)
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
				var procInfo = new ProcessStartInfo
				{
					WorkingDirectory = path,
					FileName = exeFile,
					Arguments = "",
					CreateNoWindow = false,
					LoadUserProfile = false,
					UseShellExecute = true,
					WindowStyle = ProcessWindowStyle.Normal
				};
				using (var proc = Process.Start(procInfo))
				{
					proc.WaitForExit(5);
				}
			}
		}
	}
}