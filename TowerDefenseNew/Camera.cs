using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using Zenseless.OpenTK;

namespace TowerDefenseNew
{
    internal class Camera
    {

        float ratio = 0;

        public Camera(float scale, Vector2 centerOffset)
        {
            this._scale = scale;
            this._center = this._center - centerOffset;
        }

        public Matrix4 CameraMatrix => cameraMatrix;

        public Matrix4 InvViewportMatrix { get; private set; }
        public void Draw()
        {
            GL.LoadMatrix(ref cameraMatrix);
        }
        public void Resize(int width, int height)
        {
            float whMaxDiff = 320;
            float targetRatioX = 0.3f; //maximale Ratio in X Richtung
            float targetRatioY = (width - height) > whMaxDiff ? 0.5f : 0.3f; //maximale Ratio in Y Richtung
            float whRatio = width / (float)height; //aktuelle breite zu höhe
            float hwRatio = height / (float)width; //aktuelle höhe zu breite

            if (width > height)
            {
                _windowAspectRatioY = targetRatioY;
                _windowAspectRatioX = _windowAspectRatioY / whRatio;
            }
            else {
                _windowAspectRatioX = targetRatioX;
                _windowAspectRatioY = _windowAspectRatioX / hwRatio;
            }

            float XYratio = _windowAspectRatioX / _windowAspectRatioY;
            float YXratio = _windowAspectRatioY / _windowAspectRatioX;

            Console.WriteLine($"W: {width}, H: {height},W/H: {(width / (float)height)}, X: {_windowAspectRatioX}, Y: {_windowAspectRatioY}, XY: {XYratio}, YX: {YXratio}");
            //_windowAspectRatio = height / (float)width;
            GL.Viewport(0, 0, width, height); // tell OpenGL to use the whole window for drawing
            var viewport = Transformation2d.Combine(Transformation2d.Translate(Vector2.One), Transformation2d.Scale(width / 2f, height / 2f));

            InvViewportMatrix = viewport.Inverted();
            UpdateMatrix();

        }

        public void WindowReshape(int width, int height)
        {
            /*
            // Make the projection matrix active
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            // The "Graphics" width and height contain the desired pixel resolution to keep.
            GL.Ortho(0.0, Graphics.Width, Graphics.Height, 0.0, 1.0, -1.0);
            // Calculate the proper aspect ratio to use based on window ratio
            var ratioX = width / (float)Graphics.Width;
            var ratioY = height / (float)Graphics.Height;
            var ratio = ratioX < ratioY ? ratioX : ratioY;
            // Calculate the width and height that the will be rendered to
            var viewWidth = Convert.ToInt32(Graphics.Width * ratio);
            var viewHeight = Convert.ToInt32(Graphics.Height * ratio);
            // Calculate the position, which will apply proper "pillar" or "letterbox" 
            var viewX = Convert.ToInt32((width - Graphics.Width * ratio) / 2);
            var viewY = Convert.ToInt32((height - Graphics.Height * ratio) / 2);
            // Apply the viewport and switch back to the GL_MODELVIEW matrix mode
            GL.Viewport(viewX, viewY, viewWidth, viewHeight);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();*/
        }

        public Vector2 Center
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
            var aspect = Transformation2d.Scale(_windowAspectRatioX, _windowAspectRatioY);
            var scale = Transformation2d.Scale(1f / Scale );
            

            cameraMatrix = Transformation2d.Combine(translate,scale * aspect);
        }

    }
}