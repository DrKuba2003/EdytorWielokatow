using EdytorWielokatow.Utils;
using EdytorWielokatow.Vertexes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdytorWielokatow.Edges
{
    public class EdgesList
    {

        public Edge? Head { get; set; }
        public Edge? Tail { get; set; }
        public int Count { get; private set; }

        public EdgesList()
        {
            Count = 0;
        }

        public void AddEdgeAtEnd(Edge e)
        {
            if (Count == 0)
            {
                Head = e;
            }
            else if (Tail is not null) // if not necessary
            {
                Tail.Next = e;
                e.Prev = Tail;
            }

            Tail = e;
            Count++;
        }

        public bool DeleteVertex(Vertex v)
        {
            (Edge? ePrev, Edge? eNext) = GetAdjecentEdges(v);
            if (ePrev is null || eNext is null) return true;

            Edge newEdge = new Edge(eNext.PrevVertex, ePrev.NextVertex,
                eNext.Prev, ePrev.Next);
            eNext.Prev!.Next = newEdge;
            ePrev.Next!.Prev = newEdge;

            // Change of head and tail if needed
            bool deletingHead = eNext == Head || ePrev == Head;
            bool deletingTail = eNext == Tail || ePrev == Tail;
            if (deletingHead)
                Head = newEdge;

            if (deletingTail)
            {
                if (!deletingHead)
                    Tail = newEdge;
                else
                    Tail = newEdge.Prev;
            }

            Count--;
            return false;
        }

        public void EdgeSubdivison(Edge e)
        {
            Vertex midpoint = GeometryUtils.Midpoint(e.PrevVertex, e.NextVertex);

            Edge prevEdge = new Edge(e.PrevVertex, midpoint, e.Prev, null);
            Edge nextEdge = new Edge(midpoint, e.NextVertex, prevEdge, e.Next);
            prevEdge.Next = nextEdge;

            prevEdge.Prev!.Next = prevEdge;
            nextEdge.Next!.Prev = nextEdge;

            if (e == Head)
                Head = prevEdge;

            if (e == Tail)
                Tail = nextEdge;

            Count++;
        }

        public void ReplaceEdge(Edge oldEdge, Edge newEdge)
        {
            if (oldEdge.Prev is not null)
                oldEdge.Prev.Next = newEdge;

            if (oldEdge.Next is not null)
                oldEdge.Next.Prev = newEdge;

            if (oldEdge == Head)
                Head = newEdge;

            if (oldEdge == Tail)
                Tail = newEdge;

            oldEdge.Next = null;
            oldEdge.Prev = null;
        }

        public (Edge? e1, Edge? e2) GetAdjecentEdges(Vertex v)
        {
            Edge? ePrev = null, eNext = null;
            TraverseAllList((Edge e) =>
            {
                if (e.PrevVertex == v)
                    ePrev = e;
                else if (e.NextVertex == v)
                    eNext = e;

                return ePrev is not null && eNext is not null;
            });
            return (ePrev, eNext);
        }

        public void MoveWholePolygon(Vertex vec)
        {
            TraverseAllList((Edge e) =>
            {
                e.NextVertex.X = e.NextVertex.X + vec.X;
                e.NextVertex.Y = e.NextVertex.Y + vec.Y;
                return false;
            });
        }

        public (int minX, int maxX, int minY, int maxY) CalculateBoundingBox()
        {
            (int minX, int maxX, int minY, int maxY) boundingBox =
                (int.MaxValue, int.MinValue, int.MaxValue, int.MinValue);

            if (Head is not null)
                TraverseAllList((Edge e) =>
                {
                    var v = e.NextVertex;

                    if (v.X < boundingBox.minX) boundingBox.minX = v.X;
                    if (v.X > boundingBox.maxX) boundingBox.maxX = v.X;
                    if (v.Y < boundingBox.minY) boundingBox.minY = v.Y;
                    if (v.Y > boundingBox.maxY) boundingBox.maxY = v.Y;

                    return false;
                });

            return boundingBox;
        }

        public void UnlockAllVertexes()
        {
            TraverseAllList((Edge edge) =>
            {
                edge.NextVertex.IsLocked = false;
                return false;
            });
        }

        public void TraverseAllList(Func<Edge, bool> action)
        {
            if (Head is null) return;

            Edge? e = Head;
            bool flag = false;
            do
            {
                flag = action(e);
                e = e.Next;
            } while (!flag && e is not null && e != Head);
        }

        public void DeleteAll()
        {
            Head = null;
            Tail = null;
            Count = 0;
        }
    }
}
