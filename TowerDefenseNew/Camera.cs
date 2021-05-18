using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using Zenseless.OpenTK;

namespace TowerDefenseNew
{
	internal class Camera
	{
		public Camera()
		{
		}

		public Matrix4 CameraMatrix => cameraMatrix;

		public Matrix4 InvViewportMatrix { get; private set; }

		public void Draw()
		{
			GL.LoadMatrix(ref cameraMatrix);
		}

		public void Resize(int width, int height)
		{
			GL.Viewport(0,0, width, height); // tell OpenGL to use the whole window for drawing

			//TODO: Save the view port aspect ratio for use in rendering
			_windowAspectRatio = height / (float)width;

			var viewport = Transformation2d.Combine(Transformation2d.Translate(Vector2.One), Transformation2d.Scale(width / 2f, height / 2f));
			InvViewportMatrix = viewport.Inverted();
			UpdateMatrix();
		}

		internal Vector2 Center
		{
			get => _center;
			set
			{
				_center = value;
				UpdateMatrix();
			}
		}

		internal float Scale
		{
			get => _scale;
			set
			{
				_scale = MathF.Max(0.001f, value); // avoid division by 0 and negative
				UpdateMatrix();
			}
		}

		private Matrix4 cameraMatrix = Matrix4.Identity;
		private float _scale = 1f;
		private float _windowAspectRatio = 1f;

		private Vector2 _center;

		private void UpdateMatrix()
		{
			//TODO: Implement window aspect ratio scaling
			//TODO: Implement camera scaling
			//TODO: Implement panning
			//TODO: Calculate the resulting camera matrix
			var translate = Transformation2d.Translate(-Center);
			var scale = Transformation2d.Scale(1f / Scale);
			var aspect = Transformation2d.Scale(_windowAspectRatio, 1f);
			cameraMatrix = Transformation2d.Combine(translate, scale, aspect);
		}
		
	}
}