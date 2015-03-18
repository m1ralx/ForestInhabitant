using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ForestInhabitant
{
    public class Jungle : ForestCell
    {
        private string Symbol { get; set; }
        private Point Location { get; set; }
        public override string GetSymbol()
        {
            return Symbol;
        }
        public Jungle(Point location)
            : base(location, "█")
        {
            this.Symbol = "█";
            Location = location;
        }
    }
}
