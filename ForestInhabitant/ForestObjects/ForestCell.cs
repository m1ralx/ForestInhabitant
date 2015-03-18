using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ForestInhabitant
{
    public interface ForestObject
    {
        Point GetLocation();
        string GetSymbol();
        bool Interaction(IForest forest, Inhabitant actor);
    }
    public abstract class ForestCell : ForestObject
    {
        public ForestCell(Point location, string symbol)
        {
            Location = location;
            Symbol = symbol;
        }
        private Point Location { get; set; }
        private string Symbol { get; set; }
        public virtual bool Interaction(IForest forest, Inhabitant actor)
        {
            return false;
        }
        public virtual string GetSymbol()
        {
            return Symbol;
        }
        public virtual Point GetLocation()
        {
            return Location;
        }
    }

    
    
    
}
