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

        public static Vertex operator -(Vertex v1, Vertex v2) =>
           new Vertex(v1.X - v2.X, v1.Y - v2.Y);

        public static Vertex operator *(int scalar, Vertex v1) =>
            new Vertex(v1.X * scalar, v1.Y * scalar);
        public static Vertex operator *(double scalar, Vertex v1) =>
            new Vertex(v1.X * (int)scalar, v1.Y * (int)scalar);

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
