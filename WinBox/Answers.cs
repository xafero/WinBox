using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using log4net;

namespace WinBox
{
	public static class Answers
	{
		private static readonly ILog log = LogManager.GetLogger("answers");
		
		public static void CopyReplace(string root, string src, string dst, IDictionary<string, string> config)
		{
			var sep = Path.DirectorySeparatorChar;
			var srcShort = src.Replace(root + sep, string.Empty);
			log.InfoFormat("Generating '{0}' from '{1}'...", dst, srcShort);
			var text = File.ReadAllText(src);
			var keys = config.Keys.Select(k => string.Format("%{0}%", k)).ToArray();
			text = ReplaceVars(config, keys, text);
			File.WriteAllText(dst, text);
		}

		private static string ReplaceVars(IDictionary<string, string> config, string[] keys, string text)
		{
			if (!text.Contains("%"))
				return text;
			Array.ForEach(keys, k => {
			              	if (!text.Contains(k))
			              		return;
			              	var value = config[k.Replace("%","")];
			              	value = ReplaceVars(config, keys, value);
			              	text = text.Replace(k, value);
			              });
			return text;
		}
	}
}