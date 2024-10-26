using EdytorWielokatow.Utils;
using EdytorWielokatow.Vertexes;

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

        public bool ValidateEdges(Edge prevEdge, Edge nextEdge)
        {
            (Edge edge, bool isPrev) last = (nextEdge, false);
            var roolback = new Dictionary<Vertex, PointF>();
            var queue = new Queue<(bool isPrev, Edge e)>();

            prevEdge.NextVertex.IsLocked = true;
            nextEdge.PrevVertex.IsLocked = true;

            //if ((nextEdge.GetType() != typeof(Edge) && nextEdge.Prev is not BezierEdge) ||
            //    nextEdge.Next is BezierEdge)
            //if (prevEdge.GetType() != typeof(Edge) ||
            //    prevEdge.Prev is BezierEdge)

            queue.Enqueue((false, nextEdge));
            nextEdge.NextVertex.IsLocked = nextEdge is not BezierEdge;

            queue.Enqueue((true, prevEdge));
            prevEdge.PrevVertex.IsLocked = prevEdge is not BezierEdge;

            while (queue.Count > 0)
            {
                var item = queue.Dequeue();

                var changed = item.isPrev ? item.e.NextVertex : item.e.PrevVertex;
                var changing = item.isPrev ? item.e.PrevVertex : item.e.NextVertex;

                roolback[changing] = new PointF(changing.X, changing.Y);
                item.e.ChangeVertexPos(changed, changing);

                if (item.isPrev)
                {
                    if (item.e is not BezierEdge &&
                        ((!item.e.Prev!.PrevVertex.IsLocked && item.e.Prev!.GetType() != typeof(Edge)) ||
                        item.e.Prev is BezierEdge ||
                        item.e.Prev.Prev is BezierEdge))
                    {
                        queue.Enqueue((true, item.e.Prev));
                        // zeby bylo oznaczone ze bedzie zmienianie tylko w przypadku jesli nie jest bezierem
                        item.e.Prev.PrevVertex.IsLocked = item.e.Prev is not BezierEdge;
                    }
                }
                else
                {
                    if (item.e is not BezierEdge &&
                        ((!item.e.Next!.NextVertex.IsLocked && item.e.Next!.GetType() != typeof(Edge)) ||
                        item.e.Next is BezierEdge ||
                        item.e.Next.Next is BezierEdge))
                    {
                        queue.Enqueue((false, item.e.Next));
                        item.e.Next.NextVertex.IsLocked = item.e.Next is not BezierEdge;
                    }
                }
                last = (item.e, item.isPrev);
            }

            if ((last.isPrev && !last.edge.Prev!.IsValid()) ||
                (!last.isPrev && !last.edge.Next!.IsValid()))
            {
                // Rolling back changes
                foreach (var key in roolback.Keys)
                    key.CopyData(roolback[key]);

                return true;
            }

            UnlockAllVertexes();

            return false;
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

            Edge newEdge = new Edge(ePrev.PrevVertex, eNext.NextVertex,
                ePrev.Prev, eNext.Next);
            ePrev.Prev!.Next = newEdge;
            eNext.Next!.Prev = newEdge;

            // Change of head and tail if needed
            bool deletingHead = ePrev == Head || eNext == Head;
            bool deletingTail = ePrev == Tail || eNext == Tail;
            if (deletingHead)
                Head = newEdge;

            if (deletingTail)
            {
                if (!deletingHead)
                    Tail = newEdge;
                else
                    Tail = newEdge.Prev;
            }

            if (newEdge.Prev is not BezierEdge)
            {
                var newPrevVertex = new Vertex(newEdge.PrevVertex);
                newEdge.PrevVertex = newPrevVertex;
                newEdge.Prev!.NextVertex = newPrevVertex;
            }

            if (newEdge.Next is not BezierEdge)
            {
                var newNextVertex = new Vertex(newEdge.NextVertex);
                newEdge.NextVertex = newNextVertex;
                newEdge.Next!.PrevVertex = newNextVertex;
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

            if (prevEdge.Prev is not BezierEdge)
            {
                var newPrevVertex = new Vertex(e.PrevVertex);
                prevEdge.Prev.NextVertex = newPrevVertex;
                prevEdge.PrevVertex = newPrevVertex;
            }

            if (nextEdge.Next is not BezierEdge)
            {
                var newNextVertex = new Vertex(e.NextVertex);
                nextEdge.Next.PrevVertex = newNextVertex;
                nextEdge.NextVertex = newNextVertex;
            }

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

            if (newEdge is BezierEdge)
            {
                if (oldEdge.Prev is not BezierEdge)
                    ReplaceVertex(newEdge.PrevVertex, new BezierVertex(newEdge.PrevVertex));
                if (oldEdge.Next is not BezierEdge)
                    ReplaceVertex(newEdge.NextVertex, new BezierVertex(newEdge.NextVertex));
            }
            else if (oldEdge is BezierEdge)
            {
                if (oldEdge.Prev is not BezierEdge)
                    ReplaceVertex(newEdge.PrevVertex, new Vertex(newEdge.PrevVertex));
                if (oldEdge.Next is not BezierEdge)
                    ReplaceVertex(newEdge.NextVertex, new Vertex(newEdge.NextVertex));
            }

            oldEdge.Next = null;
            oldEdge.Prev = null;
        }

        public void ReplaceVertex(Vertex oldVertex, Vertex newVertex)
        {
            (Edge? ePrev, Edge? eNext) = GetAdjecentEdges(oldVertex);

            ePrev!.NextVertex = newVertex;
            eNext!.PrevVertex = newVertex;
        }

        public (Edge? e1, Edge? e2) GetAdjecentEdges(Vertex v)
        {
            Edge? ePrev = null, eNext = null;
            TraverseAllList((Edge e) =>
            {
                if (e.PrevVertex == v)
                    eNext = e;
                else if (e.NextVertex == v)
                    ePrev = e;

                if (e is BezierEdge)
                {
                    var be = (BezierEdge)e;
                    if (be.PrevControlVertex == v ||
                        be.NextControlVertex == v)
                    {
                        ePrev = be.Prev;
                        eNext = be.Next;
                    }
                }

                return ePrev is not null && eNext is not null;
            });
            return (ePrev, eNext);
        }

        // TODO moze exceptions to za duzo, nieoptymalne
        public void MoveWholePolygon(Vertex vec, List<Vertex>? exceptions = null)
        {
            TraverseAllList((Edge e) =>
            {
                if (exceptions is null ||
                    !exceptions.Contains(e.NextVertex))
                {
                    foreach (var v in e.GetVertexesExceptPrev())
                    {
                        v.X += vec.X;
                        v.Y += vec.Y;
                    }
                }

                return false;
            });
        }

        public (float minX, float maxX, float minY, float maxY) CalculateBoundingBox()
        {
            (float minX, float maxX, float minY, float maxY) boundingBox =
                (float.MaxValue, float.MinValue, float.MaxValue, float.MinValue);

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
