using System;
using System.Linq;
using Mono.Net;

namespace WinBox
{
	public static class BootHoster
	{
		public static bool HostFile(byte[] bytes)
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