using EdytorWielokatow.Utils;
using EdytorWielokatow.Vertexes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdytorWielokatow.Edges
{
    public class BezierEdge : Edge
    {
        public const int CONTROL_RADIUS = 5;

        public static new Icon? icon =
            Icon.FromHandle(new Bitmap("Resources\\Bezier.png").GetHicon());
        public static new Rectangle rect = new Rectangle(-10, -10, 20, 20);

        public Vertex PrevControlVertex { get; set; }
        public Vertex NextControlVertex { get; set; }
        public BezierEdge(Vertex prevVert, Vertex nextVert, Vertex prevControlVert, Vertex nextControlVert,
            Edge? prev = null, Edge? next = null)
            : base(prevVert, nextVert, prev, next)
        {
            PrevControlVertex = prevControlVert;
            NextControlVertex = nextControlVert;
        }

        public BezierEdge(Edge e, Vertex prevControlVert, Vertex nextControlVert)
            : this(e.PrevVertex, e.NextVertex, prevControlVert, nextControlVert, e.Prev, e.Next)
        { }

        public override void ChangeVertexPos(Vertex changed, Vertex changing)
        {
            // changing jest tylko odblokowany w tej funkcji

            bool isPrev = changed == PrevVertex;
            var correspondingEdge = isPrev ? Prev : Next;
            if (correspondingEdge is null) return;
            BezierVertex correspondingEdgeVertex = isPrev ?
                (BezierVertex)PrevVertex :
                (BezierVertex)NextVertex;
            var continuityClass = correspondingEdgeVertex.ContinuityClass;
            if (continuityClass != ContinuityClasses.G1 &&
                continuityClass != ContinuityClasses.C1) return;

            var controlVertex = isPrev ? PrevControlVertex : NextControlVertex;
            var vec = new Vertex(
                correspondingEdgeVertex.X - correspondingEdge.GetNeighVertex(correspondingEdgeVertex).X,
                correspondingEdgeVertex.Y - correspondingEdge.GetNeighVertex(correspondingEdgeVertex).Y
                );


            changing.IsLocked = false;
            switch (continuityClass)
            {
                case ContinuityClasses.C1:
                    controlVertex.X = correspondingEdgeVertex.X + vec.X;
                    controlVertex.Y = correspondingEdgeVertex.Y + vec.Y;
                    break;
                case ContinuityClasses.G1:
                    var vecL = GeometryUtils.VectorLength(vec);
                    var L = GeometryUtils.DistB2P(correspondingEdgeVertex, controlVertex);
                    double scalar = L / vecL;

                    controlVertex.X = correspondingEdgeVertex.X + (int)Math.Round(vec.X * scalar, 0);
                    controlVertex.Y = correspondingEdgeVertex.Y + (int)Math.Round(vec.Y * scalar, 0);
                    break;
            }
        }

        public override bool IsValid(Vertex v1, Vertex v2) => true;

        public override IEnumerable<(Vertex v, bool isControl)> GetVertexesExceptPrev()
        {
            yield return (PrevControlVertex, true);
            yield return (NextControlVertex,  true);
            yield return (NextVertex, false);
        }

        public override Vertex GetNeighVertex(Vertex v)
            => v == PrevVertex ? PrevControlVertex : NextControlVertex;

        public override void Draw(Graphics g, bool useBresenham = false, Pen? p = null)
        {
            float[] dashValues = { 2, 2 };
            Pen dashPen = new Pen(p is null ? Brushes.Blue : p.Brush, 2);
            dashPen.DashPattern = dashValues;
            base.Draw(g, false, dashPen);

            g.DrawLine(dashPen,
                    PrevVertex.X, PrevVertex.Y,
                    PrevControlVertex.X, PrevControlVertex.Y);

            g.DrawLine(dashPen,
                    PrevControlVertex.X, PrevControlVertex.Y,
                    NextControlVertex.X, NextControlVertex.Y);

            g.DrawLine(dashPen,
                    NextControlVertex.X, NextControlVertex.Y,
                    NextVertex.X, NextVertex.Y);

            PrevControlVertex.Draw(g, Brushes.Magenta, CONTROL_RADIUS);
            NextControlVertex.Draw(g, Brushes.Magenta, CONTROL_RADIUS);

            GeometryUtils.Bezier(g, PrevVertex, PrevControlVertex, NextControlVertex, NextVertex, dashPen.Brush);
        }

        public override Icon? GetIcon() => icon;
        public override Rectangle GetIconRectangle() => rect;
    }
}
