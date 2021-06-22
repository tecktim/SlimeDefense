using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using Zenseless.OpenTK;

namespace TowerDefenseNew
{
    internal class Camera
    {
        public Camera(float scale, Vector2 centerOffset)
        {
            this._scale = scale;
            this._center = this._center - centerOffset;
        }

        public Matrix4 CameraMatrix => cameraMatrix;

        public Matrix4 InvViewportMatrix { get; private set; }

        int _width;
        int _height;

        public void Draw()
        {
            GL.LoadMatrix(ref cameraMatrix);
        }

        public void Resize(int width, int height)
        {
            
            GL.Viewport(0, 0, width, height); // tell OpenGL to use the whole window for drawing

            // _windowAspectRatio = height / (float)width;

            float ratioX = width / (float)height;
            float ratioY = height / (float)width;
            float ratio = ratioX < ratioY ? ratioX : ratioY;
            float newHeight = height * ratio;
            float newWidth = width * ratio;
            if (ratio < 0.56f)
            {
                _windowAspectRatio = newHeight / newWidth;
            }
            else _windowAspectRatio = (newWidth * ratio) / newHeight;


            Console.WriteLine($"{width}, {height}, {ratio}");
            var viewport = Transformation2d.Combine(Transformation2d.Translate(Vector2.One), Transformation2d.Scale(width / 2f, height / 2f));
            InvViewportMatrix = viewport.Inverted();
            GL.Ortho(0, width, height, 0, -1, 1);
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
        private float _scale { get; set; }
        private float _windowAspectRatio = 1f;

        private Vector2 _center;

        private void UpdateMatrix()
        {
            var translate = Transformation2d.Translate(Center);
            var scale = Transformation2d.Scale(1f / Scale);

            var aspect = Transformation2d.Scale(_windowAspectRatio, 1f);
           

            cameraMatrix = Transformation2d.Combine(translate, scale, aspect);
        }

    }
}