using System;
using System.Linq;

namespace WinBox
{
	public static class Defaults
	{
		public static Pack CreateVirtualBox(MachineConfig m)
		{
			var pack = new Pack
			{
				builders = {
					new Builder {
						type = "virtualbox-iso",
						vboxmanage = {
							new [] { "modifyvm", "{{.Name}}", "--memory", m.Memory+"" },
							new [] { "modifyvm", "{{.Name}}", "--vram", m.Vram+"" },
							new [] { "modifyvm", "{{.Name}}", "--cpus", m.Cpus+"" }
						},
						guest_additions_mode = "{{ user `guest_additions_mode` }}",
						guest_additions_path = "C:/users/vagrant/VBoxGuestAdditions.iso",
						guest_os_type = m.OperatingSystem,
						iso_url = "{{ user `iso_url` }}",
						iso_checksum = "{{ user `iso_checksum` }}",
						iso_checksum_type = "sha1",
						communicator = "winrm",
						headless = "{{ user `headless` }}",
						winrm_username = "vagrant",
						winrm_password = "vagrant",
						winrm_port = 5985,
						winrm_timeout = "24h",
						shutdown_command = "C:/windows/system32/sysprep/sysprep.exe /generalize /oobe /unattend:C:/Windows/Panther/Unattend/unattend.xml /quiet /shutdown",
						shutdown_timeout = "15m",
						floppy_files = new [] {
							"answer_files/win7/Autounattend.xml",
							"answer_files/win7/postunattend.xml",
							"scripts/boxstarter.ps1",
							"scripts/package.ps1",
							"scripts/Test-Command.ps1"
						}
					}
				},
				/* provisioners = {
					new Provisioner {
						type = "powershell",
						script = "scripts/provision.ps1"
					}
				}, */
				post_processors = {
					new PostProcessor {
						type = "vagrant",
						keep_input_artifact = true,
						output = "windows7-{{.Provider}}.box",
						vagrantfile_template = "vagrantfile-windows.template"
					}
				},
				variables = new Variables {
					guest_additions_mode = "attach",
					headless = "false",
					iso_checksum = "39d2e2924e186124ea44d2453069b34ef18ea45e",
					iso_url = "iso/Win7_Pro_SP1_German_x64.iso"
				}
			};
			var builder = pack.builders.First();
			foreach (var forward in m.Forwardings)
				builder.vboxmanage.Insert(0, new [] { "modifyvm", "{{.Name}}", "--natpf1", forward.ToString() });
			return pack;
		}
	}
}