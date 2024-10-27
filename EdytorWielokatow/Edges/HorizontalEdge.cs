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
                // Krawedzie: cv <-> v1 <-> v2
                ContinuityVertex v1 = (ContinuityVertex)(isPrev ? PrevVertex : NextVertex);
                Vertex v2 = isPrev ? NextVertex : PrevVertex;
                ControlVertex cv = (ControlVertex)neighEdge.GetNeighVertex(v1);

                switch (v1.ContinuityClass)
                {
                    case ContinuityClasses.C1:
                        var vec = new Vertex(v1.X - cv.X, v1.Y - cv.Y);
                        changing.X = v1.X + vec.X * 2;
                        changing.Y = v1.Y + vec.Y * 2;
                        break;
                    case ContinuityClasses.G1:
                        // jesli wektory nie sa w tym samym kierunku
                        if ((v1.X - cv.X) * (v2.X - v1.X) < 0)
                        {
                            v2.X = v1.X + (v1.X - v2.X);
                        }
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
