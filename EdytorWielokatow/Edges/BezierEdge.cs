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

        public Vertex PrevControlVertex {  get; set; }
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
            base.ChangeVertexPos(changed, changing);
        }

        public override bool IsValid(Vertex v1, Vertex v2) => true;

        public override IEnumerable<Vertex> GetVertexesExceptPrev()
        {
            yield return PrevControlVertex;
            yield return NextControlVertex;
            yield return NextVertex;
        }

        public override void Draw(Graphics g, bool useBresenham = false, Pen? p = null)
        {
            float[] dashValues = { 2, 2 };
            Pen dashPen = new Pen(p is null ? Brushes.Blue : p.Brush, 2);
            dashPen.DashPattern = dashValues;
            base.Draw(g, false, dashPen);

            g.DrawLine(dashPen,
                    PrevVertex.X, PrevVertex.Y,
                    PrevControlVertex.X, PrevControlVertex.Y);

            g.FillEllipse(Brushes.Magenta,
                        PrevControlVertex.X - CONTROL_RADIUS, PrevControlVertex.Y - CONTROL_RADIUS,
                        2 * CONTROL_RADIUS, 2 * CONTROL_RADIUS);

            g.DrawLine(dashPen,
                    PrevControlVertex.X, PrevControlVertex.Y,
                    NextControlVertex.X, NextControlVertex.Y);

            g.FillEllipse(Brushes.Magenta,
                        NextControlVertex.X - CONTROL_RADIUS, NextControlVertex.Y - CONTROL_RADIUS,
                        2 * CONTROL_RADIUS, 2 * CONTROL_RADIUS);

            g.DrawLine(dashPen,
                    NextControlVertex.X, NextControlVertex.Y,
                    NextVertex.X, NextVertex.Y);

            GeometryUtils.Bezier(g, PrevVertex, PrevControlVertex, NextControlVertex, NextVertex, dashPen.Brush);
        }

        public override Icon? GetIcon() => icon;
        public override Rectangle GetIconRectangle() => rect;
    }
}
