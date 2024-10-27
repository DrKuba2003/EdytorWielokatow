using EdytorWielokatow.Utils;
using EdytorWielokatow.Vertexes;

namespace EdytorWielokatow.Edges
{
    public class Edge
    {
        public static readonly Rectangle rect = new Rectangle(0, 0, 0, 0);

        public Edge? Prev { get; set; }
        public Edge? Next { get; set; }

        public Vertex PrevVertex { get; set; }
        public Vertex NextVertex { get; set; }

        public Edge(Vertex prevVert, Vertex nextVert, Edge? prev = null, Edge? next = null)
        {
            PrevVertex = prevVert;
            NextVertex = nextVert;
            Prev = prev;
            Next = next;
        }

        public Edge(Edge e)
        {
            PrevVertex = e.PrevVertex;
            NextVertex = e.NextVertex;
            Prev = e.Prev;
            Next = e.Next;
        }

        public virtual void ChangeVertexPos(Vertex changed, Vertex changing)
        {
            bool isPrev = changed == PrevVertex;
            var neighEdge = isPrev ? Prev : Next;
            if (neighEdge is null) return;

            if (neighEdge is BezierEdge)
            {
                ContinuityVertex bv = (ContinuityVertex)(isPrev ? PrevVertex : NextVertex);
                ControlVertex cv = (ControlVertex)neighEdge.GetNeighVertex(bv);

                var vec = new Vertex(bv.X - cv.X, bv.Y - cv.Y);
                double scalar = 2;

                if (bv.ContinuityClass == ContinuityClasses.G1)
                {
                    var vecL = GeometryUtils.VectorLength(vec);
                    if (vecL < 0.1)
                        return;
                    var L = GeometryUtils.DistB2P(PrevVertex, NextVertex);
                    scalar = L / vecL;
                }

                changing.X = (float)(bv.X + vec.X * scalar);
                changing.Y = (float)(bv.Y + vec.Y * scalar);
            }
        }

        public virtual bool IsValid(Vertex v1, Vertex v2) => true;

        public bool IsValid()
            => IsValid(PrevVertex, NextVertex);

        public virtual IEnumerable<Vertex> GetVertexesExceptPrev()
        {
            yield return NextVertex;
        }

        public Vertex GetEdgeVertex(bool prev)
            => prev ? PrevVertex : NextVertex;

        public Edge GetNeighEdge(bool prev)
            => prev ? Prev : Next;

        public virtual Vertex GetNeighVertex(Vertex v)
            => v == PrevVertex ? NextVertex : PrevVertex;

        public virtual void Draw(Graphics g, bool useBresenham = false, Pen? p = null)
        {
            if (useBresenham)
                GeometryUtils.Bresenhams(g, PrevVertex.X.Round(), PrevVertex.Y.Round(),
                            NextVertex.X.Round(), NextVertex.Y.Round(), p.Brush);
            else
                g.DrawLine(p is null ? new Pen(Brushes.Blue, 3) : p,
                        PrevVertex.X, PrevVertex.Y,
                        NextVertex.X, NextVertex.Y);

            var icon = GetIcon();
            if (icon is not null)
            {
                var midpt = GeometryUtils.Midpoint(PrevVertex, NextVertex);
                var rect = GetIconRectangle();
                rect.Offset(new Point(midpt.X.Round(), midpt.Y.Round()));
                g.DrawIcon(icon, rect);
                icon.Dispose();
            }

        }

        public virtual Icon? GetIcon() => null;
        public virtual Rectangle GetIconRectangle() => rect;
    }
}
