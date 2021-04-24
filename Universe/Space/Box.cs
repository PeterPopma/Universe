using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universe.Space
{
    class Box
    {
        double left;
        double top;
        double right;
        double bottom;

        public Box(double left, double top, double right, double bottom)
        {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
        }

        public double Left { get => left; set => left = value; }
        public double Top { get => top; set => top = value; }
        public double Right { get => right; set => right = value; }
        public double Bottom { get => bottom; set => bottom = value; }
    }
}
