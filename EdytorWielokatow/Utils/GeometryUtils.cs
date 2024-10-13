using EdytorWielokatow.Edges;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.AxHost;

namespace EdytorWielokatow.Utils
{
    public static class GeometryUtils
    {

        public enum TypeCode { Point, Edge };

        public static Vertex midpoint(Vertex a, Vertex b) =>
            new Vertex((a.X + b.X) / 2, (a.Y + b.Y) / 2);

        public static double DistBPE(Vertex pt, Edge edge)
        {
            double a = DistB2P(edge.PrevVertex, edge.NextVertex);
            double b = DistB2P(edge.PrevVertex, pt);
            double c = DistB2P(edge.NextVertex, pt);

            double s = (a + b + c) / 2;

            if (a == 0) return double.MaxValue;

            return 2 * Math.Sqrt(s * (s - a) * (s - b) * (s - c)) / a;
        }

        public static double DistB2P(Vertex p1, Vertex p2) =>
            Math.Sqrt(SquaredDistB2P(p1, p2));

        public static double SquaredDistB2P(Vertex p1, Vertex p2) =>
            (Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));

        public static bool CheckIf2PClose(Vertex p1, Vertex p2, double buffer) =>
            SquaredDistB2P(p1, p2) < Math.Pow(buffer, 2);
    }
}
