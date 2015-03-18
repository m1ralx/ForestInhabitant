using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForestInhabitant
{
    public class SimpleConsoleVisualizator : IVisualizator
    {
        public void Vizualize(ForestObject[,] map, Dictionary<string, Inhabitant> inhabitants)
        {
            Console.Clear();
            for (var y = 0; y < map.GetLength(1); y++)
            {
                for (var x = 0; x < map.GetLength(0); x++)
                    Console.Write(map[x, y].GetSymbol());
                Console.WriteLine();
            }
            PrintLegend(inhabitants);
            PrintHelp();
        }
        public void PrintHelp()
        {
            Console.WriteLine("Use arrow keys to move");
            Console.WriteLine("Escape — exit");
        }
        public void PrintLegend(Dictionary<string, Inhabitant> inhabitants)
        {
            Console.WriteLine("\n█ — Jungle\n♥ — MedKit\n* — Trap");
            foreach (var inh in inhabitants.Values.Where(inhab => inhab.GetHitPoints() > 0))
            {
                Console.WriteLine("{0} — Forest inhabitant {1} HP", inh.GetName(), inh.GetHitPoints());
            }
        }
    }
}
