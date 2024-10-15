using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdytorWielokatow.Vertexes;

namespace EdytorWielokatow.Edges
{
    public class VerticalEdge : Edge
    {

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

        public override Vertex ChangeVertexPos(Vertex changed, Vertex changing)
        {
            return new Vertex(changed.X, changing.Y, changing.IsLocked);
        }
        public override Icon? GetIcon() => icon;
        public override Rectangle GetRectangle() => rect;
    }
}
