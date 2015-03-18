using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ForestInhabitant
{
    public delegate void OnMapChange(ForestObject[,] forest, Dictionary<string, Inhabitant> inhabitants);
    public class Forest : IForest
    {
        public static Dictionary<string, Func<Point, ForestCell>> CellTypes { get; private set; }
        private Dictionary<string, Inhabitant> Inhabitants { get; set; }
        private ForestObject[,] Map { get; set; }
        public int Width { get { return Map.GetLength(0); } }
        public int Height { get { return Map.GetLength(1); } }
        public event OnMapChange OnChangeMap;

        public ForestObject[,] GetMap()
        {
            return Map;
        }
        public bool HasAliveInhabitants
        {
            get
            {
                return Inhabitants.Values.Any(inh => inh.GetHitPoints() > 0);
            }
        }
        public ForestObject this[Point p]
        {
            get
            {
                return Map[p.X, p.Y];
            }
            set
            {
                try
                {
                    Map[p.X, p.Y] = value;
                }
                catch (Exception r)
                {
                    Console.WriteLine(r);
                }
            }
        }
        public static void InitCellTypes()
        {
            CellTypes = new Dictionary<string, Func<Point, ForestCell>>();
            CellTypes["K"] = (location) => new Trap(location);
            CellTypes["L"] = (location) => new MedKit(location);
            CellTypes["0"] = (location) => new Road(location);
            CellTypes["1"] = (location) => new Jungle(location);
        }
        public Forest(ForestObject[,] map)
        {
            Map = map;
            InitCellTypes();
            Inhabitants = new Dictionary<string, Inhabitant>();
            
        }
        private void CheckOnChangeMap()
        {
            if (OnChangeMap != null)
                OnChangeMap(Map, Inhabitants);
        }
        private void RemoveInhabitant(Point location, string name)
        {
            this[location] = Inhabitants[name].LastCell;
            Inhabitants.Remove(name);
            OnChangeMap(Map, Inhabitants);
        }
        public void SpawnNewInhabitant(string name, Point location)
        {
            var inhabitant = new Inhabitant(name, location);
            this[location] = inhabitant;
            Inhabitants.Add(name, inhabitant);
            CheckOnChangeMap();
            inhabitant.OnDead += RemoveInhabitant;
        }
        private double GetLength(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }
        public bool MoveInhabitant(Point direction, Inhabitant inhabitant)
        {
            if (GetLength(new Point(0, 0), direction) > 1)
                throw new ArgumentException("Bad argument direction");
            bool resultOfMove = inhabitant.Move(this, direction);
            CheckOnChangeMap();
            return resultOfMove;
        }
        private bool CheckBounds(Point p)
        {
            return p.X >= 0 && p.X < Map.GetLength(0) && p.Y >= 0 && p.Y < Map.GetLength(1);  
        }
    }
}
