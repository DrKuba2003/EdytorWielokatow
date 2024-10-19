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
        public const int EPS = 0;

        public static new Icon? icon =
            Icon.FromHandle(new Bitmap("Resources\\Vertical.png").GetHicon());
        public static new readonly Rectangle rect = new Rectangle(-5, -10, 20, 20);

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

        public override void Draw(Graphics g, bool useBresenham = false, Brush? b = null)
        {
            base.Draw(g, useBresenham, b);
#if DEBUG
            var midpt = GeometryUtils.Midpoint(PrevVertex, NextVertex);
            g.DrawString(
                $"{PrevVertex.X - NextVertex.X}",
                SystemFonts.DefaultFont, Brushes.Black,
                new PointF(rect.X + midpt.X + 3, rect.Y + midpt.Y + 20)
                );
#endif
        }

        public override Icon? GetIcon() => icon;
        public override Rectangle GetIconRectangle() => rect;
    }
}
