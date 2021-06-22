using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using Zenseless.Patterns;

namespace Zenseless.OpenTK
{
	/// <summary>
	/// Frame buffer class that handles rendering to texture(s).
	/// </summary>
	/// <seealso cref="Disposable" />
	public class FrameBuffer : Disposable, IObjectHandle
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FrameBuffer"/> class.
		/// </summary>
		/// <exception cref="FrameBufferException">
		/// Given texture is null or texture dimensions do not match primary texture
		/// </exception>
		public FrameBuffer(bool disposesAttachments = true)
		{
			// Create an FBO object
			GL.CreateFramebuffers(1, out int handle);
			Handle = handle;
			DisposesAttachments = disposesAttachments;
		}

		/// <summary>
		/// Return the texture that is attached to the given attachment point.
		/// </summary>
		/// <param name="attachmentPoint">The attachment point where the requested texture is attached to.</param>
		/// <returns></returns>
		public Texture GetTexture(FramebufferAttachment attachmentPoint) => _attachedTextures[attachmentPoint];

		/// <summary>
		/// Handle of the currently active frame buffer. Equals 0 if the default is active
		/// </summary>
		public static int CurrentFrameBufferHandle { get; private set; } = 0;

		/// <summary>
		/// If true all attachments are disposed when the FrameBuffer is disposed.
		/// </summary>
		public bool DisposesAttachments { get; }

		/// <summary>
		/// Returns the OpenGL object handle
		/// </summary>
		public int Handle { get; }

		/// <summary>
		/// Attaches the specified texture. The FBO will try to dispose the texture when the FBO is disposed.
		/// </summary>
		/// <param name="texture">The texture to attach.</param>
		/// <param name="attachmentPoint">The attachment point to attach to.</param>
		/// <exception cref="FrameBufferException">
		/// Given texture is null or texture dimensions do not match primary texture
		/// </exception>
		public void Attach(Texture texture, FramebufferAttachment attachmentPoint)
		{
			if (texture is null) throw new ArgumentNullException(nameof(texture));
			if (0 < _attachedTextures.Count)
			{
				var firstTexture = _attachedTextures.First().Value;
				if (firstTexture.Width != texture.Width || firstTexture.Height != texture.Height)
					throw new ArgumentException($"Given Texture dimension ({texture.Width},{texture.Height}) " +
						$"do not match primary texture ({firstTexture.Width},{firstTexture.Height})");
			}
			_attachedTextures.Add(attachmentPoint, texture);
			if (DisposesAttachments)
			{
				_disposables.Add(texture);
			}
			GL.NamedFramebufferTexture(Handle, attachmentPoint, texture.Handle, 0);
			CheckFramebufferStatus();

			static bool IsDrawBuffer(FramebufferAttachment a) => Enum.IsDefined(typeof(DrawBuffersEnum), (int)a);
			var drawBuffers = _attachedTextures.Keys.Where(a => IsDrawBuffer(a)).Select(a => (DrawBuffersEnum)a).ToArray();
			GL.NamedFramebufferDrawBuffers(Handle, drawBuffers.Length, drawBuffers);
		}

		/// <summary>
		/// Attaches the specified render buffer. The FBO will try to dispose the render buffer when the FBO is disposed.
		/// </summary>
		/// <param name="renderBuffer">The render buffer to attach.</param>
		/// <param name="attachmentPoint">The attachment point to attach to.</param>
		/// <exception cref="FrameBufferException">
		/// Given texture is null or texture dimensions do not match primary texture
		/// </exception>
		public void Attach(RenderBuffer renderBuffer, FramebufferAttachment attachmentPoint)
		{
			GL.NamedFramebufferRenderbuffer(Handle, attachmentPoint, RenderbufferTarget.Renderbuffer, renderBuffer.Handle);
			_disposables.Add(renderBuffer);
		}

		/// <summary>
		/// Draw to the texture attachments of the FBO.
		/// </summary>
		/// <param name="draw">The code to draw.</param>
		public void Draw(Action draw)
		{
			//OpenTK.Graphics.OpenGL.GL.PushAttrib(OpenTK.Graphics.OpenGL.AttribMask.ViewportBit);
			var viewport = new int[4];
			GL.GetInteger(GetPName.Viewport, viewport);
			var lastFBO = CurrentFrameBufferHandle;
			GL.BindFramebuffer(FramebufferTarget.Framebuffer, Handle);
			var texture = _attachedTextures.First().Value;
			GL.Viewport(0, 0, texture.Width, texture.Height);
			CurrentFrameBufferHandle = Handle;

			draw();

			GL.BindFramebuffer(FramebufferTarget.Framebuffer, lastFBO);
			GL.Viewport(viewport[0], viewport[1], viewport[2], viewport[3]);
			//OpenTK.Graphics.OpenGL.GL.PopAttrib(); //TODO: deprecated, but needed by view port
			CurrentFrameBufferHandle = lastFBO;
		}

		/// <summary>
		/// Will be called from the default Dispose method.
		/// </summary>
		protected override void DisposeResources()
		{
			foreach (var rb in _disposables) rb.Dispose();
			GL.DeleteFramebuffer(Handle);
		}

		private readonly Dictionary<FramebufferAttachment, Texture> _attachedTextures = new();
		private readonly List<IDisposable> _disposables = new();

		private void CheckFramebufferStatus()
		{
			string status = GetStatusMessage();
			if (string.Empty == status) return;
			throw new FrameBufferException(status);
		}

		private string GetStatusMessage()
		{
			return GL.CheckNamedFramebufferStatus(Handle, FramebufferTarget.Framebuffer) switch
			{
				FramebufferStatus.FramebufferComplete => string.Empty,
				FramebufferStatus.FramebufferIncompleteAttachment => "One or more attachment points are not frame buffer attachment complete. This could mean there’s no texture attached or the format isn’t renderable. For color textures this means the base format must be RGB or RGBA and for depth textures it must be a DEPTH_COMPONENT format. Other causes of this error are that the width or height is zero or the z-offset is out of range in case of render to volume.",
				FramebufferStatus.FramebufferIncompleteDrawBuffer => "An attachment point referenced by GL.DrawBuffers() doesn’t have an attachment.",
				FramebufferStatus.FramebufferIncompleteLayerTargets => "Frame buffer Incomplete Layer Targets",
				FramebufferStatus.FramebufferIncompleteMissingAttachment => "There are no attachments.",
				FramebufferStatus.FramebufferIncompleteMultisample => "Frame buffer incomplete multi sample",
				FramebufferStatus.FramebufferIncompleteReadBuffer => "The attachment point referenced by GL.ReadBuffers() doesn’t have an attachment.",
				FramebufferStatus.FramebufferUndefined => "Frame buffer Undefined",
				FramebufferStatus.FramebufferUnsupported => "This particular FBO configuration is not supported by the implementation.",
				_ => "Status unknown. (yes, this is really bad.)",
			};
		}
	}
}
