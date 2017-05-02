using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace WinBox
{
	public class Pack
	{
		private static readonly JsonSerializerSettings cfg = new JsonSerializerSettings
		{
			Formatting = Formatting.Indented,
			NullValueHandling = NullValueHandling.Ignore,
			Converters = { new StringEnumConverter() }
		};
		
		public List<Builder> builders { get; set; }
		public List<Provisioner> provisioners { get; set; }
		[JsonProperty("post-processors")]
		public List<PostProcessor> post_processors { get; set; }
		public Variables variables { get; set; }
		
		public Pack()
		{
			builders = new List<Builder>();
			provisioners = new List<Provisioner>();
			post_processors = new List<PostProcessor>();
			variables = new Variables();
		}
		
		public override string ToString()
		{
			return JsonConvert.SerializeObject(this, cfg);
		}
	}
	
	public class Builder
	{
		public string type { get; set; }
		public List<string[]> vboxmanage { get; set; }
		public string guest_additions_mode { get; set; }
		public string guest_additions_path { get; set; }
		public GuestOS? guest_os_type { get; set; }
		public string iso_url { get; set; }
		public string iso_checksum { get; set; }
		public string iso_checksum_type { get; set; }
		public string communicator { get; set; }
		public string headless { get; set; }
		public string winrm_username { get; set; }
		public string winrm_password { get; set; }
		public int winrm_port { get; set; }
		public string winrm_timeout { get; set; }
		public string shutdown_command { get; set; }
        public string vm_name { get; set; }
        public string shutdown_timeout { get; set; }
		public string[] floppy_files { get; set; }
	    public int disk_size { get; set; }
	    public bool enable_dynamic_memory { get; set; }
	    public int ram_size { get; set; }

	    public Builder()
		{
			vboxmanage = new List<string[]>();
		}

		public void AddFloppyFile(string file)
		{
			file = file.Replace('\\', '/');
			var files = floppy_files.Concat(new [] { file });
			floppy_files = files.ToArray();
		}
	}
	
	public class Provisioner
	{
		public string type { get; set; }
		public string script { get; set; }
	}
	
	public class PostProcessor
	{
		public string type { get; set; }
		public bool keep_input_artifact { get; set; }
		public string output { get; set; }
		public string vagrantfile_template { get; set; }
	}
	
	public class Variables
	{
		public string guest_additions_mode { get; set; }
		public string headless { get; set; }
		public string iso_checksum { get; set; }
		public string iso_url { get; set; }
	}
}