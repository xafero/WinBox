using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using WinBox.Net;

namespace WinBox.Boot
{
	public class BootProgram
	{
		private static readonly object sync = new object();
		
		private static AutoResetEvent waiter;
		private static Dictionary<IPAddress, IPStatus?> scanned;
		
		public static void Main(string[] args)
		{
			waiter = new AutoResetEvent(false);
			scanned = new Dictionary<IPAddress, IPStatus?>();
			TryFind();
			waiter.WaitOne();
		}
		
		private static void TryFind()
		{
			TryFind(Networking.GetAllIPAddresses());
		}
		
		private static void TryFind(IEnumerable<IPAddress> addresses)
		{
			lock (sync)
			{
				foreach (var address in addresses)
				{
					scanned[address] = null;
					var ping = new Ping();
					ping.PingCompleted += OnPingResult;
					try { ping.SendAsync(address, address); }
					catch { ping.Dispose();	}
				}
			}
		}

		private static void OnPingResult(object sender, PingCompletedEventArgs e)
		{
			lock (sync)
			{
				var addr = (IPAddress)e.UserState;
				scanned[addr] = e.Reply.Status;
				if (e.Reply.Status == IPStatus.Success)
					OnSuccess(addr, e.Reply);
				((IDisposable)sender).Dispose();
			}
		}

		private static void OnSuccess(IPAddress addr, PingReply reply)
		{
			if (reply.Address.AddressFamily == AddressFamily.InterNetwork)
			{
				if (reply.Address.GetAddressBytes()[0] == 127)
					return;
				TryConnect(addr);
				var subAddr = Networking.GenerateRange(reply.Address).Except(scanned.Keys);
				TryFind(subAddr.ToArray());
			}
		}
		
		private static void TryConnect(IPAddress addr)
		{
			Console.WriteLine(addr);
		}
	}
}