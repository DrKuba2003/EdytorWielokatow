﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdytorWielokatow.Utils;
using EdytorWielokatow.Vertexes;

namespace EdytorWielokatow.Edges
{
    public class VerticalEdge : Edge
    {
        public const double EPS = 1;

        public static new Icon? icon =
            Icon.FromHandle(new Bitmap("Resources\\Vertical.png").GetHicon());
        public static new Rectangle rect = new Rectangle(-5, -10, 20, 20);

        public VerticalEdge(Vertex prevVert, Vertex nextVert, Edge? prev = null, Edge? next = null)
            : base(prevVert, nextVert, prev, next)
        {
            nextVert.X = prevVert.X;
        }

        public VerticalEdge(Edge e)
            : this(e.PrevVertex, e.NextVertex, e.Prev, e.Next)
        { }

        public override void ChangeVertexPos(Vertex changed, Vertex changing)
        {
            changing.X = changed.X;
        }
        public override bool IsValid(Vertex v1, Vertex v2) =>
            Math.Abs(v1.X - v2.X) <= EPS;

        public override Icon? GetIcon() => icon;
        public override Rectangle GetIconRectangle() => rect;
    }
}
