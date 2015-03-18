using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ForestInhabitant
{
    public interface IForest
    {
        void SpawnNewInhabitant(string name, Point location);
        bool MoveInhabitant(Point direction, Inhabitant inhabitant);
        ForestObject this[Point p] { get; set; }
        int Width { get; }
        int Height { get; }
        ForestObject[,] GetMap();
        event OnMapChange OnChangeMap;
        bool HasAliveInhabitants { get; }
    }
}
