using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdytorWielokatow.Vertexes;

namespace EdytorWielokatow.Edges
{
    public class Edge
    {
        public static Icon? icon { get => null; }
        public static Rectangle rect { get => new Rectangle(0, 0, 0, 0); }

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

        public virtual Vertex ChangeVertexPos(Vertex changed, Vertex changing)
        {
            return new Vertex(changing);
        }

        public virtual Icon? GetIcon() => icon;
        public virtual Rectangle GetIconRectangle() => rect;
    }
}
