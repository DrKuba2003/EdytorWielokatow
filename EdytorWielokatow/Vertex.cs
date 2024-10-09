using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace EdytorWielokatow
{
    public class Vertex
    {
        public int X { get; set; }
        public int Y { get; set; }

        //public Point pt
        //{
        //    get => new Point(X, Y);
        //}

        public Vertex(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    //public class Vertex
    //{ 
    //    public Point Pt { get; set; }

    //    public Vertex(Point pt)
    //    {
    //        Pt = pt;
    //    }
    //}

}
