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
			foreach (var key in config.OfType<string>())
			{
				var value = config[key];
				var term = string.Format("%{0}%", key);
				text = text.Replace(term, value);
			}
			File.WriteAllText(dst, text);
		}
	}
}