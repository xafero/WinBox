using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Mono.Net;

namespace WinBox
{
	public static class BootHoster
	{
		public static bool HostDirectory(string root)
		{
			var sopt = SearchOption.AllDirectories;
			var files = Directory.GetFiles(root, "*.*", sopt);
			return HostFiles(s => WriteFiles(s, files));
		}

		private static void WriteFiles(Stream stream, IEnumerable<string> files,
			ZipArchiveMode mode = ZipArchiveMode.Create,
			CompressionLevel lvl = CompressionLevel.NoCompression)
		{
			using (var zip = new ZipArchive(stream, mode))
			foreach (var file in files)
			{
				var name = Path.GetFileName(file);
				zip.CreateEntryFromFile(file, name, lvl);
			}
		}

		private static bool HostFiles(Action<Stream> byter)
		{
			const int port = 56000;
			using (var listener = new HttpListener())
			{
				listener.Prefixes.Add(string.Format("http://*:{0}/winbox/", port));
				listener.Start();
				const string tmpHostFile = "hosting.tmp";
				using (var file = File.Create(tmpHostFile))
				byter(file);
				var client = listener.GetContext();
				var headers = client.Request.Headers.OfType<string>()
				.Except(new[] { "Host", "Connection" })
				.ToDictionary(k => k, v => client.Request.Headers.Get(v));
				Console.WriteLine(string.Join(", ", headers));
				var req = client.Request;
				var rsp = client.Response;
			using (var output = rsp.OutputStream)
			using (var file = File.OpenRead(tmpHostFile))
			{
				file.CopyTo(output);
				output.Flush();
			}
			File.Delete(tmpHostFile);
			listener.Stop();
			}
			return true;
		}
	}
}