using EdytorWielokatow.Utils;
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
            Vertex midpoint = GeometryUtils.midpoint(e.PrevVertex, e.NextVertex);

            Edge newEdge = new Edge(midpoint, e.NextVertex, e, e.Next);
            e.NextVertex = midpoint;
            e.Next = newEdge;
            newEdge.Next!.Prev = newEdge;

            if (e == Tail)
                Tail = newEdge;

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
            // TODO nie jestem pewny czy garbage collector usunie sam
            Head = null;
            Tail = null;
            Count = 0;
        }
    }
}
