using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Zenseless.Patterns
{
	/// <summary>
	/// Handles loading of embedded resources from the entry assembly
	/// </summary>
	public static class Resource
	{
		/// <summary>
		/// Load the resource given by name into a string
		/// </summary>
		/// <param name="name">The name of the resource.</param>
		/// <returns>a string</returns>
		public static string LoadString(string name)
		{
			using var stream = LoadStream(name);
			using var streamReader = new StreamReader(stream);
			return streamReader.ReadToEnd();
		}

		/// <summary>
		/// Load the resource given by name into a stream
		/// </summary>
		/// <param name="name">The name of the resource.</param>
		/// <returns>a stream.</returns>
		public static Stream LoadStream(string name)
		{
			var assembly = Assembly.GetEntryAssembly();
			if (assembly is null) throw new Exception("No entry assembly found. Unmanaged code.");
			var stream = assembly.GetManifestResourceStream(name);
			if (stream is null)
			{
				var names = string.Join('\n', assembly.GetManifestResourceNames());
				throw new ArgumentException($"Could not find resource '{name}' in resources\n'{names}'");
			}
			return stream;
		}

		/// <summary>
		/// Returns all resource names that contain the given text
		/// </summary>
		/// <param name="text">Text to search for</param>
		/// <returns>A list of strings</returns>
		public static IEnumerable<string> Matches(string text)
		{
			var assembly = Assembly.GetEntryAssembly();
			if (assembly is null) throw new Exception("No entry assembly found. Unmanaged code.");
			var resourceNames = assembly.GetManifestResourceNames();
			return resourceNames.Where(name => name.Contains(text));
		}
	}
}
