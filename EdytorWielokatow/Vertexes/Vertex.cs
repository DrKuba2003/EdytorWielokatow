using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace EdytorWielokatow.Vertexes
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

        public Vertex(Vertex v)
        {
            X = v.X;
            Y = v.Y;
            IsLocked = v.IsLocked;
        }

        public static Vertex operator +(Vertex v1, Vertex v2) =>
            new Vertex(v1.X + v2.X, v1.Y + v2.Y);

        public void CopyData(Vertex v)
        {
            X = v.X;
            Y = v.Y;
            IsLocked = v.IsLocked;
        }
        public void CopyData(Point v)
        {
            X = v.X;
            Y = v.Y;
            IsLocked = false;
        }


    }

}
