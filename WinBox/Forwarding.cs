using System;

namespace WinBox
{
	public class Forwarding
	{
		public string Name { get; set; }
		public string Transport { get; set; }
		public string HostIP { get; set; }
		public int HostPort { get; set; }
		public string GuestIP { get; set; }
		public int GuestPort { get; set; }

		public Forwarding(string name, string transport, string hostIP,
		                  int hostPort, string guestIP, int guestPort)
		{
			Name = name;
			Transport = transport;
			HostIP = hostIP;
			HostPort = hostPort;
			GuestIP = guestIP;
			GuestPort = guestPort;
		}
		
		public override string ToString()
		{
			return string.Format("{0},{1},{2},{3},{4},{5}", Name, Transport, HostIP, HostPort, GuestIP, GuestPort);
		}
	}
}