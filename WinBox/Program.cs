﻿using System;
using System.IO;
using System.Reflection;
using System.Linq;
using log4net;
using log4net.Config;
using WinBox.Boot;

namespace WinBox
{
    class Program
    {
        static readonly ILog log = LogManager.GetLogger("winbox");

        public static void Main(string[] args)
        {
            BasicConfigurator.Configure();
            var config = Configs.BuildConfig(ref args);
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
            GuestOS machineGuest;
            if (!Enum.TryParse(config["guest"], true, out machineGuest))
            {
                var help = string.Join(" | ", Enum.GetNames(typeof(GuestOS)));
                log.ErrorFormat("Not valid guest type! ( {0} )", help);
                return;
            }
            machine.OperatingSystem = machineGuest;
            var machRoot = Path.Combine(templRoot, machine.OperatingSystem + "");
            var answerSrc = Path.Combine(machRoot, "unattend.xml");
            var answerDst = Path.Combine(root, "Autounattend.xml");
            var builder = config["builder"];
            var name = config["name"];
            var packMeths = typeof(Defaults).GetMethods();
            var packMeth = packMeths.FirstOrDefault(m => m.Name.EndsWith(builder.Replace("-", ""), 
                StringComparison.InvariantCultureIgnoreCase));
            if (packMeth == null)
                throw new InvalidOperationException($"No pack definition for '{builder}' found!");
            var pack = (Pack)packMeth.Invoke(null, new object[] { machine, builder, name });
            Answers.CopyReplace(templRoot, answerSrc, answerDst, config);
            pack.builders.First().AddFloppyFile(answerDst);
            foreach (var afile in Reflections.GetAssemblyFiles<BootProgram>())
                pack.builders.First().AddFloppyFile(afile);
            const string vagrantFile = "vagrantfile-windows.template";
            var vagrantSrc = Path.Combine(templRoot, vagrantFile);
            var vagrantDst = Path.Combine(root, vagrantFile);
            File.Copy(vagrantSrc, vagrantDst, overwrite: true);
            var isoStore = Path.GetFullPath(config["isoStore"]);
            var machIsoStore = Path.Combine(isoStore, machine.OperatingSystem + "");
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
            Environment.Exit(0);
        }
    }
}
