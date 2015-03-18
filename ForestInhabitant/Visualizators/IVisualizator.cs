using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForestInhabitant
{
    public interface IVisualizator
    {
        void Vizualize(ForestObject[,] map, Dictionary<string, Inhabitant> inhabitants);
    }
}
