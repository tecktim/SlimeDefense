using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace Zenseless.Patterns
{
	/// <summary>
	/// Contains helper functions for file paths
	/// </summary>
	public static class PathTools
	{
		/// <summary>
		/// Returns the full path of the main module of the current process.
		/// </summary>
		/// <returns>Full path of the main module of the current process.</returns>
		public static string GetCurrentProcessPath() => Process.GetCurrentProcess().MainModule?.FileName ?? string.Empty;

		/// <summary>
		/// Returns the output directory for the current process:
		/// a sub-directory of the directory the executable resides in 
		/// and with the name of the executable and an appended time code.
		/// </summary>
		/// <param name="timeCodeFormat">string format for DateTime</param>
		/// <returns>Output directory</returns>
		public static string GetCurrentProcessOutputDir(string timeCodeFormat = "yyyyMMdd HHmmss")
		{
			var path = GetCurrentProcessPath();
			var dir = Path.GetDirectoryName(path) ?? path;
			var name = Path.GetFileNameWithoutExtension(path);
			if(!string.IsNullOrWhiteSpace(timeCodeFormat))
			{
				name += $" {DateTime.Now.ToString(timeCodeFormat)}";
			}
			return Path.Combine(dir, name);
		}

		/// <summary>
		/// Returns the full path of the source file that contains the caller. This is the file path at the time of compile.
		/// </summary>
		/// <param name="doNotAssignCallerFilePath">Dummy default parameter. Needed for internal attribute evaluation. Do not assign.</param>
		/// <returns></returns>
		public static string GetSourceFilePath([CallerFilePath] string doNotAssignCallerFilePath = "") => doNotAssignCallerFilePath;
	}
}
