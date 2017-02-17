using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Linq;
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
			var assPlace = Assembly.GetEntryAssembly().Location;
			var appRoot = Directory.GetParent(Path.GetFullPath(assPlace)).FullName;
			var templRoot = Path.Combine(appRoot, "templates");
			log.InfoFormat("Template root => {0}", templRoot);
			var machine = new MachineConfig();
			var machRoot = Path.Combine(templRoot, machine.OperatingSystem+"");
			var answerSrc = Path.Combine(machRoot, "unattend.xml");
			var answerDst = Path.Combine(root, "Autounattend.xml");
			var builder = config["builder"];
			var pack = Defaults.CreateVirtualBox(machine, builder);
			Answers.CopyReplace(templRoot, answerSrc, answerDst);
			pack.builders.First().AddFloppyFile(answerDst);
			const string vagrantFile = "vagrantfile-windows.template";
			var vagrantSrc = Path.Combine(templRoot, vagrantFile);
			var vagrantDst = Path.Combine(root, vagrantFile);
			File.Copy(vagrantSrc, vagrantDst, overwrite: true);
			var isoStore = Path.GetFullPath(config["isoStore"]);
			var machIsoStore = Path.Combine(isoStore, machine.OperatingSystem+"");
			log.InfoFormat("ISO store => {0}", machIsoStore);
			var machIso = Shell.FindNewestFile(machIsoStore, "*.iso");
			if (string.IsNullOrWhiteSpace(machIso))
			{
				log.ErrorFormat("No ISO found!");
				return;
			}
			pack.variables.iso_url = machIso.Replace('\\', '/');
			pack.variables.iso_checksum = Shell.GetOrCreateHash(machIso);
			var packFile = Path.Combine(root, "winbox.json");
			log.InfoFormat("Generating => {0}", packFile);
			File.WriteAllText(packFile, pack.ToString());
			Shell.ExecutePacker(root, packerExe, packFile, config);
			log.InfoFormat("Have a nice day!");
			if (Debugger.IsAttached) Debugger.Break();
		}
	}
}