using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

using Sc = System.StringComparison;

namespace WinBox.Config
{
	public static class ConfigHelper
	{
		public static IDictionary<string, string> ParseToDict(ref string[] args, string prefix = "-D", char sep = '=')
		{
			var leftOvers = new List<string>();
			var dict = new Dictionary<string, string>();
			foreach (var arg in args)
			{
				if (!arg.StartsWith(prefix, Sc.InvariantCultureIgnoreCase))
				{
					leftOvers.Add(arg);
					continue;
				}
				var tmp = arg.Substring(prefix.Length).Split(new [] { sep }, 2);
				var key = tmp.First();
				var val = tmp.Last();
				dict[key] = val;
			}
			args = leftOvers.ToArray();
			return dict;
		}

		public static T[] GetEnumValues<T>()
		{
			return Enum.GetValues(typeof(T)).OfType<T>().ToArray();
		}
		
		public static IDictionary<string, string> ToStringDict(params IEnumerable[] dicts)
		{
			var dict = new Dictionary<string, string>();
			foreach (var source in dicts)
			{
				foreach (var raw in source)
				{
					var keyGetter = raw.GetType().GetProperty("Key");
					var valGetter = raw.GetType().GetProperty("Value");
					var itemer = source.GetType().GetProperties().FirstOrDefault(p => p.Name == "Item");
					var key = keyGetter == null ? raw
						: keyGetter.GetValue(raw, null);
					var val = valGetter == null ? itemer.GetValue(source, new [] { raw })
						: valGetter.GetValue(raw, null);
					dict[ToConfigString(key)] = ToConfigString(val);
				}
			}
			return dict;
		}

		public static IDictionary<string, string> Normalize(IDictionary dict, string prefix = null)
		{
			return ToStringDict(dict)
				.Select(e => new { Name = prefix + ToCamelCase(e.Key), e.Value })
				.ToDictionary(k => k.Name, v => v.Value);
		}
		
		private static string ToCamelCase(string text)
		{
			return text.Substring(0, 1).ToUpperInvariant()
				+ text.Substring(1).ToLowerInvariant();
		}
		
		public static IDictionary<string, V> InvokeToDict<K, V>(IEnumerable<K> args, Func<K, V> func,
		                                                        string prefix = null)
		{
			var dict = new Dictionary<string, V>();
			foreach (var arg in args)
			{
				var val = func(arg);
				if (IsDefault(val))
					continue;
				dict[prefix + ToConfigString(arg)] = val;
			}
			return dict;
		}

		public static IDictionary<string, object> ReflectToDict(Type type, object obj = null,
		                                                        string prefix = null)
		{
			var dict = new Dictionary<string, object>();
			var flags = (obj == null ? BindingFlags.Static : BindingFlags.Instance)
				| BindingFlags.Public | BindingFlags.FlattenHierarchy;
			var props = type.GetProperties(flags).Where(p => p.CanRead)
				.Select(p => new { p.Name, Value = p.GetValue(obj, null) });
			var fields = type.GetFields(flags)
				.Select(f => new { f.Name, Value = f.GetValue(obj) });
			var all = props.Concat(fields).Where(e => !IsDefault(e.Value));
			return all.ToDictionary(k => prefix + k.Name, v => v.Value);
		}
		
		public static IDictionary<string, object> ReflectObjToDict<T>(T obj,
		                                                              string prefix = null)
		{
			return ReflectToDict(typeof(T), obj, prefix: prefix);
		}
		
		private static bool IsDefault(object obj)
		{
			if (obj == null)
				return true;
			var type = obj.GetType();
			if (type.IsValueType)
				return Activator.CreateInstance(type).Equals(obj);
			switch (type.FullName)
			{
				case "System.String":
					var text = (string)obj;
					return string.IsNullOrEmpty(text) || text.Trim().Length == 0;
				default:
					return false;
			}
		}
		
		private static string ToConfigString(object obj)
		{
			var type = obj.GetType();
			switch (type.FullName)
			{
				default:
					return obj + string.Empty;
			}
		}
	}
}