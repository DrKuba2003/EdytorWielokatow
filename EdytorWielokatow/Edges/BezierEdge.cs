using EdytorWielokatow.Utils;
using EdytorWielokatow.Vertexes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace EdytorWielokatow.Edges
{
    public class BezierEdge : Edge
    {
        public const int CONTROL_RADIUS = 5;

        public static new Icon? icon =
            Icon.FromHandle(new Bitmap("Resources\\Bezier.png").GetHicon());
        public static new Rectangle rect = new Rectangle(-10, -10, 20, 20);

        public ControlVertex PrevControlVertex { get; set; }
        public ControlVertex NextControlVertex { get; set; }
        public BezierEdge(Vertex prevVert, Vertex nextVert, Vertex prevControlVert, Vertex nextControlVert,
            Edge? prev = null, Edge? next = null)
            : base(prevVert, nextVert, prev, next)
        {
            PrevControlVertex = new ControlVertex(prevControlVert, this);
            NextControlVertex = new ControlVertex(nextControlVert, this);
        }

        public BezierEdge(Edge e, Vertex prevControlVert, Vertex nextControlVert)
            : this(e.PrevVertex, e.NextVertex, prevControlVert, nextControlVert, e.Prev, e.Next)
        { }

        public override void ChangeVertexPos(Vertex changed, Vertex changing)
        {
            // changing jest tylko odblokowany w tej funkcji

            bool isPrev = changed == PrevVertex;
            var neighEdge = isPrev ? Prev : Next;
            if (neighEdge is null) return;
            BezierVertex sharedEdgeVertex = isPrev ?
                (BezierVertex)PrevVertex :
                (BezierVertex)NextVertex;
            var continuityClass = sharedEdgeVertex.ContinuityClass;
            if (continuityClass != ContinuityClasses.G1 &&
                continuityClass != ContinuityClasses.C1) return;

            var controlVertex = isPrev ? PrevControlVertex : NextControlVertex;
            var vec = new Vertex(
                sharedEdgeVertex.X - neighEdge.GetNeighVertex(sharedEdgeVertex).X,
                sharedEdgeVertex.Y - neighEdge.GetNeighVertex(sharedEdgeVertex).Y
                );


            changing.IsLocked = false;
            switch (continuityClass)
            {
                case ContinuityClasses.C1:
                    controlVertex.X = sharedEdgeVertex.X + vec.X;
                    controlVertex.Y = sharedEdgeVertex.Y + vec.Y;
                    break;
                case ContinuityClasses.G1:
                    var vecL = GeometryUtils.VectorLength(vec);
                    if (vecL < 0.1)
                        return;
                    var L = GeometryUtils.DistB2P(sharedEdgeVertex, controlVertex);
                    double scalar = L / vecL;

                    controlVertex.X = sharedEdgeVertex.X + (int)Math.Round(vec.X * scalar, 0);
                    controlVertex.Y = sharedEdgeVertex.Y + (int)Math.Round(vec.Y * scalar, 0);
                    break;
            }
        }

        public void ControlChangeVertexPos(Vertex controlVertex)
        {
            bool isPrev = controlVertex == PrevControlVertex;
            var vertex = isPrev ? PrevVertex : NextVertex;
            var neighEdge = isPrev ? Prev : Next;

            if (neighEdge is HorizontalEdge)
            {
                vertex.Y = controlVertex.Y;
            }
            else if (neighEdge is VerticalEdge)
            {
                vertex.X = controlVertex.X;
            }
            else if (neighEdge is FixedLengthEdge)
            {
                var vec = new Vertex(controlVertex.X - vertex.X,
                            controlVertex.Y - vertex.Y);
                var vecL = GeometryUtils.DistB2P(vertex, controlVertex);
                double scalar = 1 - ((FixedLengthEdge)neighEdge).Length / vecL;

                vertex.X += (int)(vec.X * scalar);
                vertex.Y += (int)(vec.Y * scalar);
            }
        }

        public override bool IsValid(Vertex v1, Vertex v2) => true;

        public override IEnumerable<Vertex> GetVertexesExceptPrev()
        {
            yield return PrevControlVertex;
            yield return NextControlVertex;
            yield return NextVertex;
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

            dashPen.Dispose();
        }

        public override Icon? GetIcon() => icon;
        public override Rectangle GetIconRectangle() => rect;
    }
}
