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
            /*if (width > height * 16 / 9)
            {
                _windowAspectRatioX = height / (float)width;
                _windowAspectRatioY = 1f;
            }
            if (width < height * 16 / 9)
            {
                _windowAspectRatioX = width / (float)height;
                _windowAspectRatioY = 1f;
            }
            if (height > width * 9 / 16)
            {
                _windowAspectRatioX = 1f;
                _windowAspectRatioY = width / (float)height;
            }
            if(height < width * 9 / 16)
            {
                _windowAspectRatioX = 1f;
                _windowAspectRatioY = height / (float)width;
            }*/


            // GEHT FÜR 16:9-0 und 0-9:16

            /*if (width == height * 16 / 9)
            {
                _windowAspectRatioX = height / (float)width;
                _windowAspectRatioY = 1f;
            }
            if (width > height * 16 / 9)
            {
                _windowAspectRatioX = height / (float)width;
                _windowAspectRatioY = _windowAspectRatioX * 16 / 9;
            }
            if (width < height * 16 / 9)
            {
                _windowAspectRatioX = width / (float)height;
                _windowAspectRatioY = _windowAspectRatioX * 9 / 16;
            }*/

            //wenn oben geht für die fälle muss es damit doch eig gehen...

            float xyRatio = width / (float)height;
            float yxRatio = height / (float)width;

            if (width == height * 16 / 9)
            {
                _windowAspectRatioX = height / (float)width;
                _windowAspectRatioY = 1f;
                Console.WriteLine("1");
            }
            if (width > height * 16 / 9)
            {
                _windowAspectRatioX = height / (float)width;
                _windowAspectRatioY = _windowAspectRatioX * 16 / 9;
                Console.WriteLine("2");
            }
            if (width < height * 16 / 9)
            {
                _windowAspectRatioX = width / (float)height;
                _windowAspectRatioY = _windowAspectRatioX * 9 / 16;
                Console.WriteLine("3");
            }

            Console.WriteLine($"W: {width}, H: {height}, X: {_windowAspectRatioX}, Y: {_windowAspectRatioY}");


            GL.Viewport(0, 0, width, height); // tell OpenGL to use the whole window for drawing

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
        private float _scale { get; set; } = 17f;
        private float _windowAspectRatioX = 1f;
        private float _windowAspectRatioY = 1f;

        private Vector2 _center;

        private void UpdateMatrix()
        {
            var translate = Transformation2d.Translate(Center);
            var scale = Transformation2d.Scale(1f / Scale);
            var aspect = Transformation2d.Scale(_windowAspectRatioX, _windowAspectRatioY);
            
            cameraMatrix = Transformation2d.Combine(translate, scale, aspect);
        }

    }
}