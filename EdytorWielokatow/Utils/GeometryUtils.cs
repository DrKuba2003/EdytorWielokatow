using EdytorWielokatow.Edges;
using EdytorWielokatow.Vertexes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.AxHost;

namespace EdytorWielokatow.Utils
{
    public static class GeometryUtils
    {
        public enum TypeCode { Point, Edge };

        public static Vertex Midpoint(Vertex a, Vertex b) =>
            new Vertex((a.X + b.X) / 2, (a.Y + b.Y) / 2);


        public static double DistBPE(Vertex pt, Edge edge)
        {
            return Math.Abs(DistB2P(pt, edge.PrevVertex) + DistB2P(pt, edge.NextVertex) 
                - DistB2P(edge.PrevVertex, edge.NextVertex));
        }

        //public static double DistBPE(Vertex pt, Edge edge)
        //{
        //    double a = DistB2P(edge.PrevVertex, edge.NextVertex);
        //    double b = DistB2P(edge.PrevVertex, pt);
        //    double c = DistB2P(edge.NextVertex, pt);

        //    double s = (a + b + c) / 2;

        //    if (a == 0) return double.MaxValue;

        //    return 2 * Math.Sqrt(s * (s - a) * (s - b) * (s - c)) / a;
        //}

        public static double DistB2P(Vertex p1, Vertex p2) =>
            Math.Sqrt(SquaredDistB2P(p1, p2));

        public static double SquaredDistB2P(Vertex p1, Vertex p2) =>
            (Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));

        public static bool CheckIf2PClose(Vertex p1, Vertex p2, double buffer) =>
            SquaredDistB2P(p1, p2) < Math.Pow(buffer, 2);

        // TODO przestudiowac
        public static void Bresenhams(Graphics g, int x, int y, int x2, int y2, Brush brush)
        {
            int w = x2 - x;
            int h = y2 - y;
            int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
            if (w < 0) dx1 = -1; else if (w > 0) dx1 = 1;
            if (h < 0) dy1 = -1; else if (h > 0) dy1 = 1;
            if (w < 0) dx2 = -1; else if (w > 0) dx2 = 1;
            int longest = Math.Abs(w);
            int shortest = Math.Abs(h);
            if (!(longest > shortest))
            {
                longest = Math.Abs(h);
                shortest = Math.Abs(w);
                if (h < 0) dy2 = -1; else if (h > 0) dy2 = 1;
                dx2 = 0;
            }
            int numerator = longest >> 1;
            for (int i = 0; i <= longest; i++)
            {
                // for the line width
                g.FillRectangle(brush, x-1, y, 1, 1);
                g.FillRectangle(brush, x, y-1, 1, 1);
                g.FillRectangle(brush, x, y, 1, 1);
                g.FillRectangle(brush, x+1, y, 1, 1);
                g.FillRectangle(brush, x, y+1, 1, 1);

                numerator += shortest;
                if (!(numerator < longest))
                {
                    numerator -= longest;
                    x += dx1;
                    y += dy1;
                }
                else
                {
                    x += dx2;
                    y += dy2;
                }
            }
        }
    }
}
