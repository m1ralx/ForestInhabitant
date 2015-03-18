using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ForestInhabitant
{
    public class WinFormVisualizator : IVisualizator
    {
        public MainWindow Main { get; set; }
        public List<Inhabitant> Inhabitants { get; set; }
        public WinFormVisualizator(IForest forest, List<Inhabitant> inhabitants)
        {
            Main = new MainWindow(forest, inhabitants);
            Inhabitants = inhabitants;
        }
        public void Vizualize(ForestObject[,] map, Dictionary<string, Inhabitant> inhabitants)
        {
            Main.UpdateLOut();
        }
    }
}
