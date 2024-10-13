using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdytorWielokatow.Edges
{
    public class Edge
    {
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

        }
    }
}
