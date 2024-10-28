using EdytorWielokatow.Edges;

namespace EdytorWielokatow.Vertexes
{
    public class ControlVertex : Vertex
    {
        public new const string ClassName = "CONTROLVERTEX";
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
