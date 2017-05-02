using System.Linq;

namespace WinBox
{
    public static class Defaults
    {
        public static Pack CreateHyperVIso(MachineConfig m, string builderName, string vmName)
        {
            var pack = new Pack
            {
                builders = {
                    new Builder {
                        type = builderName,
                        iso_url = "{{ user `iso_url` }}",
                        iso_checksum = "{{ user `iso_checksum` }}",
                        iso_checksum_type = "sha1",
                        disk_size = 50 * 1024,
                        enable_dynamic_memory = true,
                        floppy_files = new string[0],
                        guest_additions_mode = "{{ user `guest_additions_mode` }}",
                        guest_additions_path = "C:/Windows/System32/vmguest.iso",
                        ram_size = 2 * 1024,
                        shutdown_command = "C:/windows/system32/sysprep/sysprep.exe /generalize /oobe /unattend:C:/Windows/Panther/Unattend/unattend.xml /quiet /shutdown",
                        shutdown_timeout = "15m",
                        communicator = "winrm",
                        winrm_username = "vagrant",
                        winrm_password = "vagrant",
                        winrm_port = 5985,
                        winrm_timeout = "24h",
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
                variables = new Variables
                {
                    guest_additions_mode = "attach"
                }
            };
            var builder = pack.builders.First();
            if (!string.IsNullOrWhiteSpace(vmName))
                builder.vm_name = vmName;
            builder.vboxmanage = null;
            return pack;
        }

        public static Pack CreateVirtualBoxIso(MachineConfig m, string builderName, string vmName)
        {
            var pack = new Pack
            {
                builders = {
                    new Builder {
                        type = builderName,
                        vboxmanage = {
                            new [] { "modifyvm", "{{.Name}}", "--memory", m.Memory+"" },
                            new [] { "modifyvm", "{{.Name}}", "--vram", m.Vram+"" },
                            new [] { "modifyvm", "{{.Name}}", "--cpus", m.Cpus+"" }
                        },
                        guest_additions_mode = "{{ user `guest_additions_mode` }}",
                        guest_additions_path = "C:/users/vagrant/VBoxGuestAdditions.iso",
                        guest_os_type = PatchOSIfNeeded(m.OperatingSystem),
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
                        floppy_files = new string[0]
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
                variables = new Variables
                {
                    guest_additions_mode = "attach",
                    headless = "false"
                }
            };
            var builder = pack.builders.First();
            if (!string.IsNullOrWhiteSpace(vmName))
                builder.vm_name = vmName;
            foreach (var forward in m.Forwardings)
                builder.vboxmanage.Insert(0, new[] { "modifyvm", "{{.Name}}", "--natpf1", forward.ToString() });
            return pack;
        }

        static GuestOS PatchOSIfNeeded(GuestOS guest)
        {
            var patchAttr = Reflections.GetEnumAttr<VirtualBoxAttribute>(guest);
            if (patchAttr == null)
                return guest;
            return patchAttr.RealOS;
        }
    }
}