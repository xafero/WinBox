using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace WinBox
{
	public class Pack
	{
		private static readonly JsonSerializerSettings cfg = new JsonSerializerSettings
		{
			Formatting = Formatting.Indented,
			NullValueHandling = NullValueHandling.Ignore
		};
		
		public List<Builder> builders { get; set; }
		[JsonProperty("post-processors")]
		public List<PostProcessor> post_processors { get; set; }
		public Variables variables { get; set; }
		
		public Pack()
		{
			builders = new List<Builder>();
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
		public string guest_os_type { get; set; }
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
		public string shutdown_timeout { get; set; }
		public string[] floppy_files { get; set; }
		
		public Builder()
		{
			vboxmanage = new List<string[]>();
		}
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
		public string headless { get; set; }
		public string iso_checksum { get; set; }
		public string iso_url { get; set; }
	}
}