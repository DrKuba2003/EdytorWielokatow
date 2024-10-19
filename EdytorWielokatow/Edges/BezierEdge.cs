using EdytorWielokatow.Utils;
using EdytorWielokatow.Vertexes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdytorWielokatow.Edges
{
    public class BezierEdge : Edge
    {
        public static new Icon? icon =
            Icon.FromHandle(new Bitmap("Resources\\Bezier.png").GetHicon());
        public static new Rectangle rect = new Rectangle(-10, -10, 20, 20);

        public Vertex PrevControlVertex {  get; set; }
        public Vertex NextControlVertex { get; set; }
        public BezierEdge(Vertex prevVert, Vertex nextVert, Vertex prevControlVert, Vertex nextControlVert,
            Edge? prev = null, Edge? next = null) 
            : base(prevVert, nextVert, prev, next)
        {
            PrevControlVertex = prevControlVert;
            NextControlVertex = nextControlVert;
        }

        public BezierEdge(Edge e, Vertex prevControlVert, Vertex nextControlVert)
            : this(e.PrevVertex, e.NextVertex, prevControlVert, nextControlVert, e.Prev, e.Next)
        { }

        public override void ChangeVertexPos(Vertex changed, Vertex changing)
        {
            base.ChangeVertexPos(changed, changing);
        }

        public override bool IsValid(Vertex v1, Vertex v2) => true;

        public override Icon? GetIcon() => icon;
        public override Rectangle GetIconRectangle() => rect;
    }
}
