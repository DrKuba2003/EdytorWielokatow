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
            else if (Tail is not null) // if nie jest konieczny
            {
                Tail.Next = e;
                e.Prev = Tail;
            }

            Tail = e;
            Count++;
        }

        public void TraverseAllList(Action<Edge> action)
        {
            if (Head is null) return;

            Edge? e = Head;
            do
            {
                action(e);
                e = e.Next;
            } while (e is not null && e != Head);
        }
    }
}
