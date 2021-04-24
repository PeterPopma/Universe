using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universe.Space
{
    class Region
    {
        double offsetX;
        double offsetY;
        int layer;
        List<Star> stars;

        public Region(double offsetX, double offsetY)
        {
            this.offsetX = offsetX;
            this.offsetY = offsetY;
        }

        public Region(double offsetX, double offsetY, int layer, List<Star> stars) : this(offsetX, offsetY)
        {
            this.layer = layer;
            this.stars = stars;
        }

        public double OffsetX { get => offsetX; set => offsetX = value; }
        public double OffsetY { get => offsetY; set => offsetY = value; }
        public int Layer { get => layer; set => layer = value; }
        internal List<Star> Stars { get => stars; set => stars = value; }
    }
}
