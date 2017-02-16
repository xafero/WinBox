using System;

namespace WinBox
{
	public static class Defaults
	{
		public static Pack CreateVirtualBox()
		{
			return new Pack
			{
				builders = {
					new Builder {
						type = "virtualbox-iso",
						vboxmanage = {
							new [] { "modifyvm", "{{.Name}}", "--natpf1", "winrm,tcp,127.0.0.1,55991,,5985" },
							new [] { "modifyvm", "{{.Name}}", "--memory", "5120" },
							new [] { "modifyvm", "{{.Name}}", "--vram", "36" },
							new [] { "modifyvm", "{{.Name}}", "--cpus", "2" }
						},
						guest_os_type = "Windows7_64",
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
				post_processors = {
					new PostProcessor {
						type = "vagrant",
						keep_input_artifact = true,
						output = "windows7-{{.Provider}}.box",
						vagrantfile_template = "vagrantfile-windows.template"
					}
				},
				variables = new Variables {
					headless = "false",
					iso_checksum = "39d2e2924e186124ea44d2453069b34ef18ea45e",
					iso_url = "iso/Win7_Pro_SP1_German_x64.iso"
				}
			};
		}
	}
}