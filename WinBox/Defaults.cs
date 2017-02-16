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
						type = "virtualbox-iso"
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