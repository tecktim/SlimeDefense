using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerDefenseNew.GameObjects
{
    interface IReadOnlyCircle
    {
        Vector2 Center { get; }
        float Radius { get; }
    }
}
