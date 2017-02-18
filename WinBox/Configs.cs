using System;
using System.Configuration;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using WinBox.Config;

using Env = System.Environment;
using EnvTgt = System.EnvironmentVariableTarget;
using SpecialFolder = System.Environment.SpecialFolder;

namespace WinBox
{
	public static class Configs
	{
		public static IDictionary<string, string> BuildConfig(ref string[] args)
		{
			var all = new IEnumerable[] {
				// Inject framework information...
				ConfigHelper.ReflectObjToDict(Env.OSVersion, prefix: "o"),
				ConfigHelper.ReflectToDict(typeof(Env), prefix: "n")
					.Where(e => !e.Key.Contains("StackTrace")),
				ConfigHelper.InvokeToDict(ConfigHelper.GetEnumValues<SpecialFolder>(),
				                          o => Env.GetFolderPath(o), prefix: "p"),
				ConfigHelper.ReflectObjToDict(DateTime.Now, prefix: "t"),
				// Inject environment variables...
				ConfigHelper.Normalize(Env.GetEnvironmentVariables(EnvTgt.Machine),	prefix: "e"),
				ConfigHelper.Normalize(Env.GetEnvironmentVariables(EnvTgt.User), prefix: "e"),
				ConfigHelper.Normalize(Env.GetEnvironmentVariables(EnvTgt.Process),	prefix: "e"),
				// Inject application configuration...
				ConfigurationManager.AppSettings,
				// and inject start parameters...
				ConfigHelper.ParseToDict(ref args)
			};
			// Combine all settings into one!
			return ConfigHelper.ToStringDict(all);
		}
	}
}