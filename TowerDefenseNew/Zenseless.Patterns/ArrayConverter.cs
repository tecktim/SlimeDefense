using System;
using System.Runtime.InteropServices;

namespace Zenseless.Patterns
{
	/// <summary>
	/// Reinterprets the data inside an array as a different data type 
	/// </summary>
	public static class ArrayConverter
	{
		/// <summary>
		/// Converts a byte array into an array of a given destination data type
		/// </summary>
		/// <typeparam name="T">The destination data type</typeparam>
		/// <param name="source">The byte array to convert</param>
		/// <param name="byteOffset">offset inside the source byte array</param>
		/// <param name="destinationCount">count of array elements of the destination data type</param>
		/// <returns>An array of destination type</returns>
		public static T[] FromByteArray<T>(this byte[] source, int byteOffset, int destinationCount) where T : struct
		{
			T[] destination = new T[destinationCount];
			GCHandle handle = GCHandle.Alloc(destination, GCHandleType.Pinned);
			try
			{
				IntPtr pointer = handle.AddrOfPinnedObject();
				Marshal.Copy(source, byteOffset, pointer, destinationCount * Marshal.SizeOf<T>()); // need to use Marshalling because of data type conversion
				return destination;
			}
			finally
			{
				if (handle.IsAllocated)
					handle.Free();
			}

		}

		/// <summary>
		/// Converts a given array into an array of floats
		/// </summary>
		/// <typeparam name="SourceType">The data type of the source array</typeparam>
		/// <param name="source">The source array</param>
		/// <param name="byteOffset">offset inside the source byte array</param>
		/// <returns>An array of floats</returns>
		public static float[] ToFloatArray<SourceType>(this SourceType[] source, int byteOffset = 0) where SourceType : struct
		{
			var destination = new float[source.Length * Marshal.SizeOf<SourceType>() / 4];
			GCHandle handle = GCHandle.Alloc(source, GCHandleType.Pinned);
			try
			{
				IntPtr pointer = handle.AddrOfPinnedObject();
				Marshal.Copy(pointer + byteOffset, destination, 0, destination.Length); // need to use Marshalling because of data type conversion
				return destination;
			}
			finally
			{
				if (handle.IsAllocated)
					handle.Free();
			}
		}
	}
}
