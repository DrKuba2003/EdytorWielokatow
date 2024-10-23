using EdytorWielokatow.Edges;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdytorWielokatow.Vertexes
{
    public class ControlVertex : Vertex
    {
        public BezierEdge Edge { get; set; }

        public ControlVertex(float x, float y, BezierEdge edge, bool isLocked = false) 
            : base(x, y, isLocked)
        {
            Edge = edge;
        }

        public ControlVertex(Vertex v, BezierEdge edge)
            : this(v.X, v.Y, edge, false)
        {
        }
    }
}
