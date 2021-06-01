using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TowerDefenseNew.Interfaces;

namespace TowerDefenseNew.GameObjects
{
	internal class Rect : IReadOnlyRectangle
	{
		public Rect(float minX, float minY, float sizeX, float sizeY)
		{
			MinX = minX;
			MinY = minY;
			SizeX = sizeX;
			SizeY = sizeY;
		}

		public float MinX { get; set; }
		public float MinY { get; set; }
		public float MaxX => MinX + SizeX;
		public float MaxY => MinY + SizeY;
		public float SizeX { get; }
		public float SizeY { get; }
	}
}
