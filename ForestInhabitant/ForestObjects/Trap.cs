using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ForestInhabitant
{
    public class Trap : ForestCell
    {
        private string Symbol { get; set; }
        private Point Location { get; set; }

        public Trap(Point location)
            : base(location, "*")
        {
            Symbol = "*";
            Location = location;
        }

        public override string GetSymbol()
        {
            return Symbol;
        }

       
        public override bool Interaction(IForest forest, Inhabitant actor)
        {
            forest[Location] = actor;
            actor.ChangeHp(-1);
            forest[actor.GetLocation()] = actor.LastCell;
            actor.LastCell = this;
            return true;
        }
    }
}
