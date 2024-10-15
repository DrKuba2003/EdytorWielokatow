﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdytorWielokatow.Vertexes;

namespace EdytorWielokatow.Edges
{
    public class HorizontalEdge : Edge
    {
        public static new Icon? icon = 
            Icon.FromHandle(new Bitmap("Resources\\Horizontal.png").GetHicon());
        public static new Rectangle rect = new Rectangle(-10, -5, 20, 20);

        public HorizontalEdge(Vertex prevVert, Vertex nextVert, Edge? prev = null, Edge? next = null)
            : base(prevVert, nextVert, prev, next)
        {
            nextVert.Y = prevVert.Y;
        }

        public HorizontalEdge(Edge e)
            : this(e.PrevVertex, e.NextVertex, e.Prev, e.Next)
        { }

        public override Vertex ChangeVertexPos(Vertex changed, Vertex changing)
        {
            return new Vertex(changing.X, changed.Y, changing.IsLocked);
        }
        public override Icon? GetIcon() => icon;
        public override Rectangle GetRectangle() => rect;
    }
}
