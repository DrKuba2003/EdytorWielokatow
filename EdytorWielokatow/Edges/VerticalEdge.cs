﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdytorWielokatow.Edges
{
    public class VerticalEdge : Edge
    {
        public VerticalEdge(Vertex prevVert, Vertex nextVert, Edge? prev = null, Edge? next = null) 
            : base(prevVert, nextVert, prev, next)
        {
        }
    }
}
