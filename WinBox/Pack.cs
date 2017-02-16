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