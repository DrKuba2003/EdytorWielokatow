using EdytorWielokatow.Utils;
using EdytorWielokatow.Vertexes;

namespace EdytorWielokatow.Edges
{
    public class HorizontalEdge : Edge
    {
        public const int EPS = 0;

        public static new Icon? icon =
            Icon.FromHandle(new Bitmap("Resources\\Horizontal.png").GetHicon());
        public static new readonly Rectangle rect = new Rectangle(-10, -5, 20, 20);

        public HorizontalEdge(Vertex prevVert, Vertex nextVert, Edge? prev = null, Edge? next = null)
            : base(prevVert, nextVert, prev, next)
        {
            nextVert.Y = prevVert.Y;
        }

        public HorizontalEdge(Edge e)
            : this(e.PrevVertex, e.NextVertex, e.Prev, e.Next)
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
                }
            }
            changing.Y = changed.Y;
        }

        public override bool IsValid(Vertex v1, Vertex v2) =>
            Math.Abs(v1.Y - v2.Y) <= EPS;

        public override void Draw(Graphics g, bool useBresenham = false, Pen? p = null)
        {
            base.Draw(g, useBresenham, p);
#if DEBUG
            var midpt = GeometryUtils.Midpoint(PrevVertex, NextVertex);
            g.DrawString(
                $"{PrevVertex.Y - NextVertex.Y}",
                SystemFonts.DefaultFont, Brushes.Black,
                new PointF(rect.X + midpt.X + 3, rect.Y + midpt.Y + 20)
                );
#endif
        }

        public override Icon? GetIcon() => icon;
        public override Rectangle GetIconRectangle() => rect;
    }
}
