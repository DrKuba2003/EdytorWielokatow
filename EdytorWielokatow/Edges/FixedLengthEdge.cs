using EdytorWielokatow.Utils;
using EdytorWielokatow.Vertexes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdytorWielokatow.Edges
{
    public class FixedLengthEdge : Edge
    {
        public static new Icon? icon = 
            Icon.FromHandle(new Bitmap("Resources\\FixedLength.png").GetHicon());
        public static new Rectangle rect = new Rectangle(-10, -10, 20, 20);

        double Length { get; }

        public FixedLengthEdge(Vertex prevVert, Vertex nextVert, double length, Edge? prev = null, Edge? next = null)
            : base(prevVert, nextVert, prev, next)
        {
            Length = length;
            NextVertex.CopyData(ChangeVertexPos(PrevVertex, NextVertex));
        }

        public FixedLengthEdge(Edge e, double length)
            : this(e.PrevVertex, e.NextVertex, length, e.Prev, e.Next)
        { }

        public override Vertex ChangeVertexPos(Vertex changed, Vertex changing)
        {
            var vec = new Vertex(changed.X - changing.X,
                        changed.Y - changing.Y);
            var vecL = GeometryUtils.DistB2P(changing, changed);
            double scalar = 1 - Length / vecL;

            return new Vertex(changing.X + (int)(vec.X * scalar),
                 changing.Y + (int)(vec.Y * scalar),
                 changing.IsLocked);
        }

        public override Icon? GetIcon() => icon;
        public override Rectangle GetIconRectangle() => rect;
    }
}
