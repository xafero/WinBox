using System;
using System.IO;
using log4net;

namespace WinBox
{
	public static class Answers
	{
		private static readonly ILog log = LogManager.GetLogger("answers");
		
		public static void CopyReplace(string root, string src, string dst)
		{
			var sep = Path.DirectorySeparatorChar;
			var srcShort = src.Replace(root + sep, string.Empty);
			log.InfoFormat("Generating '{0}' from '{1}'...", dst, srcShort);
			var text = File.ReadAllText(src);
			File.WriteAllText(dst, text);
		}
	}
}