using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace WinBox
{
	public static class Reflections
	{
		public static IEnumerable<string> GetAssemblyFiles<T>()
		{
			foreach (var ass in GetAssemblies<T>()
			         .Where(a => !a.GlobalAssemblyCache))
				yield return Path.GetFullPath(ass.Location);
		}
		
		private static IEnumerable<Assembly> GetAssemblies<T>()
		{
			var type = typeof(T);
			var ass = type.Assembly;
			yield return ass;
			var refs = ass.GetReferencedAssemblies();
			foreach (var refName in refs)
				yield return Assembly.Load(refName);
		}
	}
}