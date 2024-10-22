using EdytorWielokatow.Utils;
using EdytorWielokatow.Vertexes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdytorWielokatow.Edges
{
    public class FixedLengthEdge : Edge
    {
        public const double EPS = 1;

        public static new Icon? icon =
            Icon.FromHandle(new Bitmap("Resources\\FixedLength.png").GetHicon());
        public static new readonly Rectangle rect = new Rectangle(-10, -10, 20, 20);

        public int Length { get; }

        public FixedLengthEdge(Vertex prevVert, Vertex nextVert, int length, Edge? prev = null, Edge? next = null)
            : base(prevVert, nextVert, prev, next)
        {
            Length = length;
            ChangeVertexPos(PrevVertex, NextVertex);
        }

        public FixedLengthEdge(Edge e, int length)
            : this(e.PrevVertex, e.NextVertex, length, e.Prev, e.Next)
        { }

        public override void ChangeVertexPos(Vertex changed, Vertex changing)
        {
            bool isPrev = changed == PrevVertex;
            var neighEdge = isPrev ? Prev : Next;
            if (neighEdge is null) return;

            if (neighEdge is BezierEdge)
            {
                BezierVertex bv = (BezierVertex)(isPrev ? PrevVertex : NextVertex);
                ControlVertex cv = (ControlVertex)neighEdge.GetNeighVertex(bv);

                var vec = new Vertex(bv.X - cv.X, bv.Y - cv.Y);

                switch (bv.ContinuityClass)
                {
                    case ContinuityClasses.C1:
                        changing.X = bv.X + vec.X;
                        changing.Y = bv.Y + vec.Y;
                        break;
                    case ContinuityClasses.G1:
                        var vecL = GeometryUtils.VectorLength(vec);
                        if (vecL < 0.1)
                            return;
                        var L = GeometryUtils.DistB2P(PrevVertex, NextVertex);
                        double scalar = L / vecL;

                        changing.X = bv.X + (int)Math.Round(vec.X * scalar, 0);
                        changing.Y = bv.Y + (int)Math.Round(vec.Y * scalar, 0);
                        break;
                }
            }
            else
            {
                var vec = new Vertex(changed.X - changing.X,
                            changed.Y - changing.Y);
                var vecL = GeometryUtils.DistB2P(changing, changed);
                double scalar = 1 - Length / vecL;

                changing.X += (int)(vec.X * scalar);
                changing.Y += (int)(vec.Y * scalar);
            }
        }

        public override bool IsValid(Vertex v1, Vertex v2) =>
            Math.Abs(GeometryUtils.DistB2P(v1, v2) - Length) < EPS;

        public override void Draw(Graphics g, bool useBresenham = false, Pen? p = null)
        {
            base.Draw(g, useBresenham, p);
#if DEBUG
            var midpt = GeometryUtils.Midpoint(PrevVertex, NextVertex);
            int d = (Length - (int)GeometryUtils.DistB2P(PrevVertex, NextVertex));
            g.DrawString($"{d}", SystemFonts.DefaultFont, Brushes.Black,
                new PointF(rect.X + midpt.X + 3, rect.Y + midpt.Y + 20));
#endif
        }

        public override Icon? GetIcon() => icon;
        public override Rectangle GetIconRectangle() => rect;
    }
}
