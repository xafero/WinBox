using System;

namespace WinBox
{
	public enum GuestOS
	{
		Windows7,
		Windows7_64,
		Windows81,
		Windows81_64,
		Windows2012_64,
		Windows10,
		Windows10_64,
		[VirtualBox(Windows2012_64)] Windows2016_64
	}
	
	public class VirtualBoxAttribute : Attribute
	{
		public GuestOS RealOS { get; private set; }
		
		public VirtualBoxAttribute(GuestOS realOS)
		{
			RealOS = realOS;
		}
	}
}