﻿using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using log4net;
using log4net.Config;

namespace WinBox
{
	class Program
	{
		private static readonly ILog log = LogManager.GetLogger("winbox");
		
		public static void Main(string[] args)
		{
			BasicConfigurator.Configure();
			var config = ConfigurationManager.AppSettings;
			var root = Path.GetFullPath(config["root"] ?? Environment.CurrentDirectory);
			log.InfoFormat("Root folder => {0}", root);
			var packerExe = Path.Combine(root, "packer.exe");
			log.InfoFormat("Looking for => {0}", packerExe);
			if (!File.Exists(packerExe))
				Shell.DownloadPacker(root, config);
			Shell.ExecutePacker(packerExe, "shit.json", config);
			log.InfoFormat("Have a nice day!");
			if (Debugger.IsAttached)
				Debugger.Break();
		}
	}
}