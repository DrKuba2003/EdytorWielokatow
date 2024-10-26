using EdytorWielokatow.Utils;
using EdytorWielokatow.Vertexes;

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
                var vecL = GeometryUtils.VectorLength(vec);
                if (vecL < 0.1)
                    return;
                double scalar = Length / vecL;

                changing.X = (float)(bv.X + vec.X * scalar);
                changing.Y = (float)(bv.Y + vec.Y * scalar);
            }
            else
            {
                var vec = new Vertex(changed.X - changing.X,
                            changed.Y - changing.Y);
                var vecL = GeometryUtils.VectorLength(vec);
                if (vecL < 0.1)
                    return;
                double scalar = 1 - Length / vecL;

                changing.X += (int)(vec.X * scalar);
                changing.Y += (int)(vec.Y * scalar);
            }
        }

        public override bool IsValid(Vertex v1, Vertex v2) =>
            Math.Abs(GeometryUtils.DistB2P(v1, v2) - Length) < EPS;

        public override Icon? GetIcon() => icon;
        public override Rectangle GetIconRectangle() => rect;
    }
}
