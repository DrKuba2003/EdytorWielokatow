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
        public bool IsLocked { get; set; }

        public Vertex(int x, int y, bool isLocked = false)
        {
            X = x;
            Y = y;
            IsLocked = isLocked;
        }

        public static Vertex operator +(Vertex v1, Vertex v2) =>
            new Vertex(v1.X + v2.X, v1.Y + v2.Y);
            

    }

}
