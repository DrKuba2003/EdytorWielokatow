using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdytorWielokatow.Utils
{
    public static class Utils
    {
        public static (double, double) AddDoubleTuples(params (double X, double Y)[] ds)
        {
            double X = 0, Y = 0;
            foreach (var d in ds)
            {
                X += d.X;
                Y += d.Y;
            }
            return (X, Y);
        }
    }
}
