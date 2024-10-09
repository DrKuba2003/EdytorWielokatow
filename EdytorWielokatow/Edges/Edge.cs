using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdytorWielokatow.Edges
{
    public class Edge
    {
        public Vertex Start { get; set; }
        public Vertex End { get; set; }

        public Edge(Vertex start, Vertex end)
        {
            Start = start;
            End = end;
        }
    }
}
