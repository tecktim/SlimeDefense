using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerDefenseNew.Interfaces
{
    public interface IParticle
    {
        float Age { get; }
        Vector2 Location { get; }
    }
}
