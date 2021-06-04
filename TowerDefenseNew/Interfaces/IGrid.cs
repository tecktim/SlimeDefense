using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerDefenseNew.Grid
{
	public interface IGrid : IReadOnlyGrid
	{
		new CellType this[int column, int row] { set; get; }
	}
}
