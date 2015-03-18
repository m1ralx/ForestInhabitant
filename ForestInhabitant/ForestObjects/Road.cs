using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ForestInhabitant
{
    public class Road : ForestCell
    {
        public string Symbol { get; set; }
        public Point Location { get; set; }
        public string Name { get; set; }
        public override string GetSymbol()
        {
            return Symbol;
        }
        public Road(Point location)
            : base(location, " ")
        {
            Symbol = " ";
            Location = location;
        }

        public override bool Interaction(IForest forest, Inhabitant actor)
        {
            forest[Location] = actor;
            //wood[actor.GetLocation()] = Wood.CellTypes["0"](actor.GetLocation());
            forest[actor.GetLocation()] = actor.LastCell;
            actor.LastCell = new Road(Location); //!!!!!!!!!!!!!!!!!! 
            return true;
        }

    }
}
