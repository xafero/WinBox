using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Mono.Net;

namespace WinBox
{
	public static class BootHoster
	{
		public static bool HostFiles(params string[] files)
		{
			using (var mem = new MemoryStream())
			{
				var mode = ZipArchiveMode.Create;
				using (var zip = new ZipArchive(mem, mode))
				{
					var lvl = CompressionLevel.NoCompression;
					foreach (var file in files)
					{
						var name = Path.GetFileName(file);
						zip.CreateEntryFromFile(file, name, lvl);
					}
				}
				return HostBytes(mem.ToArray());
			}
		}

		private static bool HostBytes(byte[] bytes)
		{
			const int port = 56000;
			var listener = new HttpListener();
			listener.Prefixes.Add(string.Format("http://*:{0}/winbox/", port));
			listener.Start();
			var client = listener.GetContext();
			var headers = client.Request.Headers.OfType<string>()
				.Except(new [] { "Host", "Connection" })
				.ToDictionary(k => k, v => client.Request.Headers.Get(v));
			Console.WriteLine(string.Join(", ", headers));
			var req = client.Request;
			var rsp = client.Response;
			using (var output = rsp.OutputStream)
			{
				output.Write(bytes, 0, bytes.Length);
				output.Flush();
			}
			listener.Stop();
			return true;
		}
	}
}