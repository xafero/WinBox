using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace WinBox.Net
{
	public static class Networking
	{
		public static bool TryDownloadFile(this WebClient client, string address, string file)
		{
			try
			{
				client.DownloadFile(address, file);
				return true;
			}
			catch
			{
				return false;
			}
		}
		
		public static bool TryConnect(this TcpClient client, IPEndPoint endpoint)
		{
			try
			{
				client.Connect(endpoint);
				return true;
			}
			catch
			{
				return false;
			}
		}

		public static IEnumerable<IPAddress> GenerateRange(IPAddress address)
		{
			var bytes = address.GetAddressBytes();
			for (var i = 1; i <= 255; i++)
			{
				bytes[3] = (byte)i;
				yield return new IPAddress(bytes);
			}
		}

		public static IPHostEntry Resolve(IPAddress address)
		{
			try
			{
				return Dns.GetHostEntry(address);
			}
			catch
			{
				return null;
			}
		}

		private static IEnumerable<T> GetAdresses<T>(Func<IPInterfaceProperties, IEnumerable<T>> func)
		{
			foreach (var interf in NetworkInterface.GetAllNetworkInterfaces())
			{
				if (interf.OperationalStatus != OperationalStatus.Up)
					continue;
				var props = interf.GetIPProperties();
				var addresses = func(props);
				foreach (var address in addresses)
					yield return address;
			}
		}
		
		public static IEnumerable<IPAddress> GetAllIPAddresses()
		{
			return GetAdresses(p => p.AnycastAddresses).Select(ToIPAddress)
				.Concat(GetAdresses(p => p.DhcpServerAddresses).Select(ToIPAddress))
				.Concat(GetAdresses(p => p.DnsAddresses).Select(ToIPAddress))
				.Concat(GetAdresses(p => p.GatewayAddresses).Select(ToIPAddress))
				.Concat(GetAdresses(p => p.MulticastAddresses).Select(ToIPAddress))
				.Concat(GetAdresses(p => p.UnicastAddresses).Select(ToIPAddress))
				.Concat(GetAdresses(p => p.WinsServersAddresses).Select(ToIPAddress))
				.Distinct();
		}
		
		private static IPAddress ToIPAddress(IPAddress addr)
		{
			return addr;
		}
		
		private static IPAddress ToIPAddress(IPAddressInformation info)
		{
			return info.Address;
		}
		
		private static IPAddress ToIPAddress(GatewayIPAddressInformation info)
		{
			return info.Address;
		}
	}
}