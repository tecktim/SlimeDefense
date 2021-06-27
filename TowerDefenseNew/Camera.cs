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
            _scale = scale;
            _center -= centerOffset;
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
            float targetRatioX = 0.3f; //maximale Breitenratio sodass immer alles angezeigt werden kann
            float targetRatioY = (width - height) > whMaxDiff ? 0.5f : 0.3f; //maximale Höhenratio sodass immer alles angezeigt werden kann
            float whRatio = width / (float)height; //aktuelle breite zu höhe
            float hwRatio = height / (float)width; //aktuelle höhe zu breite

            if (width > height)
            {
                _windowAspectRatioY = targetRatioY;
                _windowAspectRatioX = _windowAspectRatioY / whRatio;
            }
            else
            {
                _windowAspectRatioX = targetRatioX;
                _windowAspectRatioY = _windowAspectRatioX / hwRatio;
            }
            GL.Viewport(0, 0, width, height); //use the whole window for drawing
            var viewport = Transformation2d.Combine(Transformation2d.Translate(Vector2.One), Transformation2d.Scale(width / 2f, height / 2f));
            InvViewportMatrix = viewport.Inverted();
            UpdateMatrix();

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
        private float _scale { get; set; }
        private float _windowAspectRatioX = 1f;
        private float _windowAspectRatioY = 1f;

        private Vector2 _center;

        private void UpdateMatrix()
        {
            var translate = Transformation2d.Translate(Center);
            var aspect = Transformation2d.Scale(_windowAspectRatioX, _windowAspectRatioY);
            var scale = Transformation2d.Scale(1f / Scale);


            cameraMatrix = Transformation2d.Combine(translate, scale * aspect);
        }

    }
}