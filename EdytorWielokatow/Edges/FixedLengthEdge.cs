using EdytorWielokatow.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdytorWielokatow.Edges
{
    public class FixedLengthEdge : Edge
    {
        double LENGTH { get; init; }
        public FixedLengthEdge(Vertex prevVert, Vertex nextVert, Edge? prev = null, Edge? next = null) 
            : base(prevVert, nextVert, prev, next)
        {
            LENGTH = GeometryUtils.DistB2P(prevVert, nextVert);
        }

        public FixedLengthEdge(Edge e)
            : this(e.PrevVertex, e.NextVertex, e.Prev, e.Next)
        { }

        public override void ChangeVertexPos(Vertex changed, Vertex changing)
        {
            var vec = new Vertex(changed.X - changing.X,
                        changed.Y - changing.Y);
            var vecL = GeometryUtils.DistB2P(changing, changed);
            double scalar = 1 - LENGTH / vecL;

            changing.X += (int)(vec.X * scalar);
            changing.Y += (int)(vec.Y * scalar);
        }


    }
}
