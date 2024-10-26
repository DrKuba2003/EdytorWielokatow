using EdytorWielokatow.Utils;
using EdytorWielokatow.Vertexes;

namespace EdytorWielokatow.Edges
{
    public class HorizontalEdge : Edge
    {
        public const int EPS = 0;

        public static readonly Icon? icon =
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
                ContinuityVertex bv = (ContinuityVertex)(isPrev ? PrevVertex : NextVertex);
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

        public override Icon? GetIcon() => icon;
        public override Rectangle GetIconRectangle() => rect;
    }
}
