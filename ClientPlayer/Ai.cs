using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using ForestSolverPackages;

namespace ForestInhabitant
{
    public enum CellTypes
    {
        NotVisited,
        Trap,
        Wall,
        Empty
    }

    public class Ai
    {
        public Point Location;
        public Point LastPoint;
        //private Func<Point, Inhabitant, bool> _move;
        //private Inhabitant Man { get; set; }
        private Point Destination { get; set; }
        //private Dictionary<Point, CellTypes> VisitedPoints { get; set; }
        public CellTypes[,] Map { get; set; }
        private int Hp;
        private int Width { get; set; }
        private int Height { get; set; }
        private Queue<Point> Queue { get; set; }

        public Ai(Point location, Point destination, int width, int height, int hp)
        {
            //VisitedPoints = new Dictionary<Point, CellTypes>{{man.GetLocation(), CellTypes.Empty}};
            //Man = man;
            Destination = destination;
            //_move = move;
            Width = width;
            Height = height;
            Map = new CellTypes[Width, Height];
            Queue = new Queue<Point>();
            Hp = hp;
            Location = location;
        }

        private IEnumerable<Point> GetVisitedNeighbours(Point current)
        {
            var directions = new[] { new Point(1, 0), new Point(-1, 0), new Point(0, 1), new Point(0, -1) };
            return directions.Select(dir => new Point(current.X + dir.X, current.Y + dir.Y)).Where(p => InField(p) && Map[p.X, p.Y] == CellTypes.Empty);
        }
        private List<Point> GetPathToNotVisitedCell(Point location, Point target)
        {
            var queue = new Queue<Point>();
            queue.Enqueue(location);
            var visited = new HashSet<Point>();
            var parents = new Dictionary<Point, Point>();
            var last = new Point(-1, -1);

            while (queue.Any())
            {
                var current = queue.Dequeue();
                if (visited.Contains(current))
                    continue;
                visited.Add(current);
                if (Math.Abs(GetLength(current, target) - 1.0) < 1e-6)
                {
                    last = current;
                    parents[target] = current;
                    break;
                }
                    
                foreach (var nei in GetNeighbours(current))
                {
                    if (visited.Contains(nei))
                        continue;
                    queue.Enqueue(nei);
                    parents[nei] = current;
                }
            }
            var answer = new List<Point>();
            var parent = target;
            while (true)
            {
                answer.Add(parent);
                try
                {
                    parent = parents[parent];
                
                }
                catch (Exception)
                {
                }
                if (GetLength(parent, location) == 0)
                    break;
            }
            answer.Reverse();
            return answer;
        }
        private List<Point> GetPathToVisitedCell(Point location, Point target)
        {
            var queue = new Queue<Point>();
            queue.Enqueue(location);
            var visited = new HashSet<Point>();
            var parents = new Dictionary<Point, Point>();
            while (queue.Any())
            {
                var current = queue.Dequeue();
                if (visited.Contains(current))
                    continue;
                visited.Add(current);
                if (current == target)
                    break;
                foreach (var nei in GetVisitedNeighbours(current))
                {
                    queue.Enqueue(nei);
                    parents[nei] = current;
                }
            }
            var answer = new List<Point>();
            var parent = target;
            while (true)
            {
                answer.Add(parent);
                parent = parents[parent];
                if (parent == location)
                    break;
            }
            answer.Reverse();
            return answer;

        }

        private List<Point> GetNeighbours(Point location)
        {
            var directions = new[] { new Point(1, 0), new Point(-1, 0), new Point(0, 1), new Point(0, -1) };
            var res = directions
                .Select(dir => new Point(location.X + dir.X, location.Y + dir.Y))
                .Where(InField)
                .Where(nei => Map[nei.X, nei.Y] == CellTypes.Empty || Map[nei.X, nei.Y] == CellTypes.Trap)
                .ToList();
            return res;
        }

        private double GetLength(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }
        private void FillQueue()
        {
            var directions = new[] { new Point(1, 0), new Point(-1, 0), new Point(0, 1), new Point(0, -1) };
            var pointsList = directions.Select(direction => new Point(Location.X + direction.X, Location.Y + direction.Y)).ToList();
            var queue = new Queue<Point>(pointsList.Where(p => InField(p) && Map[p.X, p.Y] == CellTypes.NotVisited));
            foreach (var p in Queue)
                queue.Enqueue(p);
            Queue = queue;
        }

        public Point GetNextMove()
        {
            FillQueue();
            if (!Queue.Any())
            {
                //Console.WriteLine("A");
                var next = GetPathToVisitedCell(Location, Destination).First();
                //_move(next, Man);
                LastPoint = new Point(Location.X + next.X, Location.Y + next.Y);
                return next;
            }
            Point target;
            while (true)
            {
                var location = Location;
                var list = Queue.ToList();
                list.Sort((p1, p2) => GetLength(location, p1).CompareTo(GetLength(p2, location)));

                //list.Sort((p1, p2) => GetLength(Destination, p1).CompareTo(GetLength(p2, Destination)));

                Queue = new Queue<Point>(list.Where(p => Map[p.X, p.Y] != CellTypes.Wall));
                target = Queue.Dequeue();
                if (GetLength(target, location) > 1)
                {
                    //target = Queue.First(p => Map[p.X, p.Y] == CellTypes.NotVisited);
                    
                    var next = GetPathToNotVisitedCell(location, target).First();
                    var dir = new Point(next.X - location.X, next.Y - location.Y);
                    //_move(dir, Man);
                    //Console.WriteLine("B");

                    LastPoint = new Point(Location.X + dir.X, Location.Y + dir.Y);
                    return dir;
                } 
                if (Map[target.X, target.Y] == CellTypes.NotVisited)
                    break;
            }
            if (Map[target.X, target.Y] == CellTypes.NotVisited)
            {
                //var health = Hp;
                var direction = new Point(target.X - Location.X, target.Y - Location.Y);
                if (GetLength(direction, new Point(0, 0)) > 1)
                {
                    var nextPoint = GetPathToNotVisitedCell(Location, target).First();
                    direction = new Point(nextPoint.X - Location.X, nextPoint.Y - Location.Y);
                    Queue.Enqueue(target);
                }
                //Console.WriteLine("C");

                LastPoint = new Point(Location.X + direction.X, Location.Y + direction.Y);
                return direction;
                //var result = _move(direction, Man);
                //if (!result)
                //{
                //    Map[target.X, target.Y] = CellTypes.Wall;
                //}
                //else
                //{
                //    if (Man.GetHitPoints() < health)
                //        Map[Man.GetLocation().X, Man.GetLocation().Y] = CellTypes.Trap;
                //    else
                //        Map[Man.GetLocation().X, Man.GetLocation().Y] = CellTypes.Empty;
                //}
            }
            return new Point(0, 0);
        }

        private bool InField(Point target)
        {
            return target.X >= 0 && target.X < Width && target.Y >= 0 && target.Y < Height;
        }
    }
}