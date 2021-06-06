using OpenTK.Mathematics;

namespace TowerDefenseNew.GameObjects
{
    internal class Explosion : GameObject
    {
        public float NormalizedAnimationTime { get; private set; } = 0f;
        public float AnimationLength { get; }

        public Explosion(Vector2 center, float radius, float animationLength) : base(center, radius)
        {
            AnimationLength = animationLength;
        }

        public void Update(float frameTime)
        {
            NormalizedAnimationTime += frameTime / AnimationLength;
            if (NormalizedAnimationTime >= 1f)
            {
                IsAlive = false;
            }
            NormalizedAnimationTime %= 1f;
        }
    }
}
