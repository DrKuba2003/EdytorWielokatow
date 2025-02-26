﻿using EdytorWielokatow.Utils;
using EdytorWielokatow.Vertexes;

namespace EdytorWielokatow.Edges
{
    public class BezierEdge : Edge
    {
        public new const string ClassName = "BEZIER";
        public const int CONTROL_RADIUS = 5;

        public static readonly Icon? icon =
            Icon.FromHandle(new Bitmap("Resources\\Bezier.png").GetHicon());
        public static new Rectangle rect = new Rectangle(-10, -10, 20, 20);

        public ControlVertex PrevControlVertex { get; set; }
        public ControlVertex NextControlVertex { get; set; }
        public BezierEdge(Vertex prevVertex, Vertex nextVertex, Vertex prevControlVertex, Vertex nextControlVertex,
            Edge? prev = null, Edge? next = null)
            : base(prevVertex, nextVertex, prev, next)
        {
            PrevControlVertex = new ControlVertex(prevControlVertex, this);
            NextControlVertex = new ControlVertex(nextControlVertex, this);
        }
        public BezierEdge(Vertex prevVertex, Vertex nextVertex, Edge? prev = null, Edge? next = null)
            : base(prevVertex, nextVertex, prev, next)
        {
            var midpoint = GeometryUtils.Midpoint(prevVertex, nextVertex);
            PrevControlVertex = new ControlVertex(midpoint.X, midpoint.Y + 50, this);
            NextControlVertex = new ControlVertex(midpoint.X, midpoint.Y - 50, this);
        }

        public BezierEdge(Edge e, Vertex prevControlVertex, Vertex nextControlVertex)
            : this(e.PrevVertex, e.NextVertex, prevControlVertex, nextControlVertex, e.Prev, e.Next) { }

        public BezierEdge(Edge e) : this(e.PrevVertex, e.NextVertex, e.Prev, e.Next) { }

        public override void ChangeVertexPos(Vertex changed, Vertex changing)
        {
            // changing jest tylko odblokowany w tej funkcji
            // Krawedzie: neighVertex <neighEdge> sharedVertex <(this)> controlVertex

            bool isPrev = changed == PrevVertex;
            var neighEdge = isPrev ? Prev : Next;
            if (neighEdge is null) return;
            ContinuityVertex sharedVertex = isPrev ?
                (ContinuityVertex)PrevVertex :
                (ContinuityVertex)NextVertex;
            var continuityClass = sharedVertex.ContinuityClass;
            if (continuityClass != ContinuityClasses.G1 &&
                continuityClass != ContinuityClasses.C1) return;

            var controlVertex = isPrev ? PrevControlVertex : NextControlVertex;
            var neighVertex = neighEdge.GetNeighVertex(sharedVertex);
            var vec = new Vertex(
                sharedVertex.X - neighVertex.X,
                sharedVertex.Y - neighVertex.Y
                );


            changing.IsLocked = false;
            double scalar =
                continuityClass == ContinuityClasses.C1 &&
                neighVertex is not ControlVertex ?
                0.5 : 1;

            if (continuityClass == ContinuityClasses.G1)
            {
                // odwracanie wektora controlVertex - sharedVertex zeby byla spelniona ciaglosc
                if (neighEdge is HorizontalEdge)
                {
                    controlVertex.Y = sharedVertex.Y;

                    if ((sharedVertex.X - controlVertex.X) * (neighVertex.X - sharedVertex.X) < 0)
                        controlVertex.X = sharedVertex.X + (sharedVertex.X - controlVertex.X);

                    return;
                }
                else if (neighEdge is VerticalEdge)
                {
                    controlVertex.X = sharedVertex.X;

                    if ((sharedVertex.Y - controlVertex.Y) * (neighVertex.Y - sharedVertex.Y) < 0)
                        controlVertex.Y = sharedVertex.Y + (sharedVertex.Y - controlVertex.Y);
                    
                    return;
                }

                var vecL = GeometryUtils.VectorLength(vec);
                if (vecL < 0.1)
                    return;
                var L = GeometryUtils.DistB2P(sharedVertex, controlVertex);
                scalar = L / vecL;
            }

            controlVertex.X = (float)(sharedVertex.X + vec.X * scalar);
            controlVertex.Y = (float)(sharedVertex.Y + vec.Y * scalar);
        }

        public void ControlChangeVertexPos(ControlVertex controlVertex)
        {
            bool isPrev = controlVertex == PrevControlVertex;
            ContinuityVertex vertex = (ContinuityVertex)(isPrev ? PrevVertex : NextVertex);
            var neighEdge = isPrev ? Prev : Next;

            if (vertex.ContinuityClass != ContinuityClasses.G0)
            {
                if (neighEdge is HorizontalEdge)
                {
                    vertex.Y = controlVertex.Y;
                }
                else if (neighEdge is VerticalEdge)
                {
                    vertex.X = controlVertex.X;
                }
            }

            if (vertex.ContinuityClass == ContinuityClasses.C1 &&
                neighEdge is FixedLengthEdge)
            {
                var vec = new Vertex(controlVertex.X - vertex.X,
                            controlVertex.Y - vertex.Y);
                var vecL = GeometryUtils.DistB2P(vertex, controlVertex);
                double scalar = 1 - ((FixedLengthEdge)neighEdge).Length / vecL / 2;

                vertex.X += (float)(vec.X * scalar);
                vertex.Y += (float)(vec.Y * scalar);
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
