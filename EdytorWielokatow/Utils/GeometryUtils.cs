
using EdytorWielokatow.Edges;
using EdytorWielokatow.Vertexes;
using System.Numerics;

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

        public static double DistB2P(Vertex p1, Vertex p2) =>
            Math.Sqrt(SquaredDistB2P(p1, p2));

        public static double SquaredDistB2P(Vertex p1, Vertex p2) =>
            (Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        public static double VectorLength(Vertex v) =>
            Math.Sqrt((Math.Pow(v.X, 2) + Math.Pow(v.Y, 2)));

        public static bool CheckIf2PClose(Vertex p1, Vertex p2, int buffer) =>
            SquaredDistB2P(p1, p2) < Math.Pow(buffer, 2);

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
                g.FillRectangle(brush, x, y, 1, 1);
                if (longest == Math.Abs(w))
                {
                    g.FillRectangle(brush, x, y - 1, 1, 1);
                    g.FillRectangle(brush, x, y + 1, 1, 1);
                }
                else
                {
                    g.FillRectangle(brush, x - 1, y, 1, 1);
                    g.FillRectangle(brush, x + 1, y, 1, 1);
                }

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

        public static void Bezier(Graphics g, Vertex V0, Vertex V1, Vertex V2, Vertex V3, Brush brush)
        {
            Vector2 A1 = 3 * (V1.Vector2 - V0.Vector2);
            Vector2 A2 = 3 * (V2.Vector2 - 2 * V1.Vector2 + V0.Vector2);
            Vector2 A3 = V3.Vector2 - 3 * V2.Vector2 + 3 * V1.Vector2 - V0.Vector2;

            double dist = DistB2P(V0, V1) + DistB2P(V1, V2) + DistB2P(V2, V3);
            float d = (float)(1 / (2 * dist));
            float d2 = (float)Math.Pow(d, 2);
            float d3 = (float)Math.Pow(d, 3);

            Vector2 P0 = V0.Vector2;
            Vector2 P1 = d3 * A3 + d2 * A2 + d * A1;
            Vector2 P2 = 6 * d3 * A3 + 2 * d2 * A2;
            Vector2 P3 = 6 * d3 * A3;

            double t = 0;
            var P0old = new PointF(P0.X, P0.Y);
            while (t < 1)
            {
                P0 += P1;
                P1 += P2;
                P2 += P3;

                var P0f = new PointF(P0.X, P0.Y);
                g.DrawLine(new Pen(brush, 3), P0old, P0f);
                P0old = P0f;

                t += d;
            }
        }

        //public static void plotLine(int x0, int y0, int x1, int y1)
        //{
        //    int dx = abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
        //    int dy = -abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
        //    int err = dx + dy, e2;                                  /* error value e_xy */

        //    for (; ; )
        //    {                                                        /* loop */
        //        setPixel(x0, y0);
        //        e2 = 2 * err;
        //        if (e2 >= dy)
        //        {                                       /* e_xy+e_x > 0 */
        //            if (x0 == x1) break;
        //            err += dy; x0 += sx;
        //        }
        //        if (e2 <= dx)
        //        {                                       /* e_xy+e_y < 0 */
        //            if (y0 == y1) break;
        //            err += dx; y0 += sy;
        //        }
        //    }
        //}

        //public static void plotLineAA(int x0, int y0, int x1, int y1)
        //{             /* draw a black (0) anti-aliased line on white (255) background */
        //    int sx = x0 < x1 ? 1 : -1, sy = y0 < y1 ? 1 : -1, x2;
        //    long dx = abs(x1 - x0), dy = abs(y1 - y0), err = dx * dx + dy * dy;
        //    long e2 = err == 0 ? 1 : 0xffff7fl / sqrt(err);     /* multiplication factor */

        //    dx *= e2; dy *= e2; err = dx - dy;                       /* error value e_xy */
        //    for (; ; )
        //    {                                                 /* pixel loop */
        //        setPixelAA(x0, y0, abs(err - dx + dy) >> 16);
        //        e2 = err; x2 = x0;
        //        if (2 * e2 >= -dx)
        //        {                                            /* x step */
        //            if (x0 == x1) break;
        //            if (e2 + dy < 0xff0000l) setPixelAA(x0, y0 + sy, (e2 + dy) >> 16);
        //            err -= dy; x0 += sx;
        //        }
        //        if (2 * e2 <= dy)
        //        {                                             /* y step */
        //            if (y0 == y1) break;
        //            if (dx - e2 < 0xff0000l) setPixelAA(x2 + sx, y0, (dx - e2) >> 16);
        //            err += dx; y0 += sy;
        //        }
        //    }
        //}

        //public static void plotLineWidth(int x0, int y0, int x1, int y1, float wd)
        //{                                    /* plot an anti-aliased line of width wd */
        //    int dx = abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
        //    int dy = abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
        //    int err = dx - dy, e2, x2, y2;                           /* error value e_xy */
        //    float ed = dx + dy == 0 ? 1 : sqrt((float)dx * dx + (float)dy * dy);

        //    for (wd = (wd + 1) / 2; ;)
        //    {                                    /* pixel loop */
        //        setPixelColor(x0, y0, max(0, 255 * (abs(err - dx + dy) / ed - wd + 1)));
        //        e2 = err; x2 = x0;
        //        if (2 * e2 >= -dx)
        //        {                                            /* x step */
        //            for (e2 += dy, y2 = y0; e2 < ed * wd && (y1 != y2 || dx > dy); e2 += dx)
        //                setPixelColor(x0, y2 += sy, max(0, 255 * (abs(e2) / ed - wd + 1)));
        //            if (x0 == x1) break;
        //            e2 = err; err -= dy; x0 += sx;
        //        }
        //        if (2 * e2 <= dy)
        //        {                                             /* y step */
        //            for (e2 = dx - e2; e2 < ed * wd && (x1 != x2 || dx < dy); e2 += dy)
        //                setPixelColor(x2 += sx, y0, max(0, 255 * (abs(e2) / ed - wd + 1)));
        //            if (y0 == y1) break;
        //            err += dx; y0 += sy;
        //        }
        //    }
        //}

    }
}
