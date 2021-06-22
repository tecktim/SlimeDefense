using OpenTK.Graphics.OpenGL4;
using System;
using Zenseless.Patterns;

namespace Zenseless.OpenTK
{
	/// <summary>
	/// A vertex array class for interpreting buffer data.
	/// </summary>
	public class VertexArray : Disposable, IObjectHandle
	{
		/// <summary>
		/// Create a new vertex array object
		/// </summary>
		public VertexArray()
		{
			GL.CreateVertexArrays(1, out int handle);
			Handle = handle;
		}

		/// <summary>
		/// Returns the OpenGL object handle
		/// </summary>
		public int Handle { get; }

		/// <summary>
		/// Binds the given buffer as an element buffer to the vertex array object
		/// </summary>
		/// <param name="buffer">the buffer to bind</param>
		public void BindIndices(Buffer buffer)
		{
			GL.VertexArrayElementBuffer(Handle, buffer.Handle);
		}

		/// <summary>
		/// Activate the vertex array
		/// </summary>
		public void Bind()
		{
			GL.BindVertexArray(Handle); // activate vertex array; from now on state is stored;
		}

		/// <summary>
		/// Binds a buffer as an attribute.
		/// </summary>
		/// <param name="attributeLocation">binding location</param>
		/// <param name="buffer">the buffer with the attribute data</param>
		/// <param name="baseTypeCount">Each buffer element consists of a type that is made up of multiple base types like for Vector3 the base type count is 3.</param>
		/// <param name="elementByteSize">Byte size of one buffer element</param>
		/// <param name="type">Element base type</param>
		/// <param name="perInstance">Is this attribute per instance</param>
		/// <param name="normalized">Should the input data be normalized</param>
		/// <param name="offset">Offset into the buffer</param>
		public void BindAttribute(int attributeLocation, Buffer buffer, int baseTypeCount, int elementByteSize, VertexAttribType type, bool perInstance = false, bool normalized = false, int offset = 0)
		{
			if (-1 == attributeLocation) throw new ArgumentException("Invalid attribute location");
			GL.EnableVertexArrayAttrib(Handle, attributeLocation);
			GL.VertexArrayVertexBuffer(Handle, attributeLocation, buffer.Handle, new IntPtr(offset), elementByteSize);
			GL.VertexArrayAttribBinding(Handle, attributeLocation, attributeLocation);
			GL.VertexArrayAttribFormat(Handle, attributeLocation, baseTypeCount, type, normalized, 0);
			if (perInstance)
			{
				GL.VertexArrayBindingDivisor(Handle, attributeLocation, 1);
			}
		}

		/// <summary>
		/// Will be called from the default Dispose method.
		/// Implementers should dispose all their resources her.
		/// </summary>
		protected override void DisposeResources() => GL.DeleteVertexArray(Handle);
	}
}