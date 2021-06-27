using OpenTK.Mathematics;

namespace TowerDefenseNew.Interfaces
{
    public interface IParticle
    {
        float Age { get; }
        Vector2 Location { get; }
    }
}
