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
		
		public Pack()
		{
			builders = new List<Builder>();
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
}