using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeighborProximity
{
    internal class City
    {
        public string Name { get; set; }
        public double LocationX { get; set; }
        public double LocationY { get; set; }
        public double LocationZ { get; set; }

        public City(string name, double locationX, double locationY, double locationZ)
        {
            Name = name;
            LocationX = locationX;
            LocationY = locationY;
            LocationZ = locationZ;
        }
    }
}