using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace ForestInhabitant
{
    public class Program
    {
        //[STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var moves = new Dictionary<ConsoleKey, Point>
            {
                { ConsoleKey.LeftArrow, new Point(-1, 0)},
                { ConsoleKey.RightArrow, new Point(1, 0)},
                { ConsoleKey.UpArrow, new Point(0, -1)},
                { ConsoleKey.DownArrow, new Point(0, 1)}
            };
            var forest = Loader.LoadForest(args[0]);

            IVisualizator visualisator = new SimpleConsoleVisualizator();
            forest.OnChangeMap += visualisator.Vizualize;

            var inhabitants = new List<Inhabitant>();

            forest.SpawnNewInhabitant("goden", new Point(7, 2));
            //forest.SpawnNewInhabitant("lol", new Point(1, 1));
            var goden = (Inhabitant)forest[new Point(7, 2)];
            //var goden = forest[new Point(1, 1)];
            inhabitants.Add(goden);
            //forest.SpawnNewInhabitant("mirana", new Point(1, 1));
            //var mirana = forest[new Point(1, 1)];
            //inhabitants.Add((Inhabitant)mirana);
            
            //Application.Run(winForm.Main);           


            //NewGame(moves, forest, inhabitants);
            var dest = new Point(5, 4);
            //var dest = new Point(7, 4);
            //var dest = new Point(1, 1);
            var ai = new Ai(goden, dest, forest.MoveInhabitant, forest.Width, forest.Height);
            while (true)
            {
                var location = goden.GetLocation();
                ai.MoveInhabitantToDestination();
                if (goden.GetLocation() == dest || goden.GetHitPoints() == 0)
                    break;
                //if (location != goden.GetLocation())
                //    Console.ReadKey();
            }
        }

        private static void NewGame(Dictionary<ConsoleKey, Point> moves, IForest forest, List<Inhabitant> inhabitants)
        {
            while (forest.HasAliveInhabitants)
            {
                var currentInhabitant = inhabitants.First(inh => inh.GetHitPoints() > 0);
                var inputKey = Console.ReadKey().Key;
                try
                {
                    forest.MoveInhabitant(moves[inputKey], currentInhabitant);
                }
                catch
                {
                    var exit = new Dictionary<ConsoleKey, Action<int>>{
                        { ConsoleKey.Escape, System.Environment.Exit},
                    };
                    try
                    {
                        exit[inputKey](0);
                    }
                    catch
                    {
                    }
                }
            }
        }
    }
}
