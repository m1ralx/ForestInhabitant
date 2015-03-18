using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ForestInhabitant
{
    public class MedKit : ForestCell
    {
        private string Symbol { get; set; }
        private Point Location { get; set; }
        
        public MedKit(Point location)
            : base(location, "♥")
        {
            Symbol = "♥";
            Location = location;
        }

        public override string GetSymbol()
        {
            return Symbol;
        }

        public override bool Interaction(IForest forest, Inhabitant actor)
        {
            forest[Location] = actor;
            actor.ChangeHp(1);
            actor.LastCell = new Road(Location);
            forest[actor.GetLocation()] = new Road(actor.GetLocation());
            return true;
        }
    }
}
