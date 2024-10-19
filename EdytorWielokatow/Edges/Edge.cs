using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdytorWielokatow.Utils;
using EdytorWielokatow.Vertexes;

namespace EdytorWielokatow.Edges
{
    public class Edge
    {
        public static Icon? icon { get => null; }
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

        public virtual void ChangeVertexPos(Vertex changed, Vertex changing) { }

        public virtual bool IsValid(Vertex v1, Vertex v2) => true;

        public bool IsValid()
            => IsValid(PrevVertex, NextVertex);

        public virtual void Draw(Graphics g, bool useBresenham = false, Brush? b = null)
        {
            if (useBresenham)
                GeometryUtils.Bresenhams(g, PrevVertex.X, PrevVertex.Y,
                            NextVertex.X, NextVertex.Y, b);
            else
                g.DrawLine(new Pen(b is null ? Brushes.Blue : b, 3),
                        PrevVertex.X, PrevVertex.Y,
                        NextVertex.X, NextVertex.Y);

            var icon = GetIcon();
            if (icon is not null)
            {
                var midpt = GeometryUtils.Midpoint(PrevVertex, NextVertex);
                var rect = GetIconRectangle();
                rect.Offset(new Point(midpt.X, midpt.Y));
                g.DrawIcon(icon, rect);
                icon.Dispose();
            }

        }

        public virtual Icon? GetIcon() => icon;
        public virtual Rectangle GetIconRectangle() => rect;
    }
}
