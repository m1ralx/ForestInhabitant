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
        Inhabitant SpawnNewInhabitant(string name, Point location, int hp);
        bool MoveInhabitant(Point direction, Inhabitant inhabitant);
        ForestObject this[Point p] { get; set; }
        int Width { get; }
        int Height { get; }
        ForestObject[,] GetMap();
        event OnMapChange OnChangeMap;
        bool HasAliveInhabitants { get; }
        void RemoveInhabitant(Point location, string name);
    }
}
