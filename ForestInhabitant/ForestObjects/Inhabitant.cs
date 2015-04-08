using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForestInhabitant
{
    public delegate void OnInhabitantDead(Point location, string name);
    public class Inhabitant : ForestObject
    {
        public ForestObject LastCell { get; set; }
        public event OnInhabitantDead OnDead;
        private string Symbol 
        { 
            get
            {
                return Name.First().ToString();
            } 
            set
            {
                Symbol = value;
            } 
        }
        private Point Location { get; set; }
        private string Name { get; set; }
        private int HitPoints { get; set; }

        public Inhabitant(string name, Point location, int hp)
        {
            LastCell = new Road(location);
            Location = location;
            Name = name;
            HitPoints = hp;
        }

        public string GetSymbol()
        {
            return Symbol;
        }
        public string GetName()
        {
            return Name;
        }
        public Point GetLocation()
        {
            return Location;
        }
        public int GetHitPoints()
        {
            return HitPoints;
        }

        public void ChangeHp(int point)
        {
            HitPoints += point;
        }

        public bool Interaction(IForest forest, Inhabitant actor)
        {
            forest[Location] = actor;
            forest[actor.GetLocation()] = actor.LastCell;
            actor.LastCell = this;
            return true;
        }
        
        private void SetLocation(Point location)
        {
            Location = location;
        }
        private void DoNothing(Point location)
        {
        }
        public bool Move(IForest forest, Point direction)
        {
            var targetPoint = new Point(Location.X + direction.X, Location.Y + direction.Y);
            var neighbour = forest[targetPoint];
            var resultOfMove = neighbour.Interaction(forest, this);
            while (resultOfMove)
            {
                Location = targetPoint;
                break;
            }
            while (HitPoints <= 0)
            {
                OnDead(Location, Name);
                break;
            }
            return resultOfMove;
        }
        

    }
}
