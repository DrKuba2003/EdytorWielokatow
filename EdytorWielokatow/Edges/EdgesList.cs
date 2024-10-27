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
            var queue = new Queue<(Edge e, bool isPrev)>();

            prevEdge.NextVertex.IsLocked = true;
            nextEdge.PrevVertex.IsLocked = true;

            queue.Enqueue((nextEdge, false));
            nextEdge.NextVertex.IsLocked = nextEdge is not BezierEdge;

            queue.Enqueue((prevEdge, true));
            prevEdge.PrevVertex.IsLocked = prevEdge is not BezierEdge;

            while (queue.Count > 0)
            {
                (Edge e, bool isPrev) = queue.Dequeue();

                var changed = isPrev ? e.NextVertex : e.PrevVertex;
                var changing = isPrev ? e.PrevVertex : e.NextVertex;

                roolback[changing] = new PointF(changing.X, changing.Y);
                e.ChangeVertexPos(changed, changing);

                if (e is not BezierEdge &&
                        ((!e.GetNeighEdge(isPrev).GetEdgeVertex(isPrev).IsLocked && e.GetNeighEdge(isPrev).GetType() != typeof(Edge)) ||
                        e.GetNeighEdge(isPrev) is BezierEdge ||
                        e.GetNeighEdge(isPrev).GetNeighEdge(isPrev) is BezierEdge))
                {
                    queue.Enqueue((e.GetNeighEdge(isPrev), isPrev));
                    // zeby bylo oznaczone ze bedzie zmienianie tylko w przypadku jesli nie jest bezierem
                    e.GetNeighEdge(isPrev).GetEdgeVertex(isPrev).IsLocked = e.GetNeighEdge(isPrev) is not BezierEdge;
                }

                last = (e, isPrev);
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
            else if (Tail is not null)
            {
                Tail.Next = e;
                e.Prev = Tail;
            }

            Tail = e;
            Count++;
        }

        public bool DeleteVertex(Vertex v)
        {
            (Edge? prevEdge, Edge? nextEdge) = GetAdjecentEdges(v);
            if (prevEdge is null || nextEdge is null) return true;

            Edge newEdge = new Edge(prevEdge.PrevVertex, nextEdge.NextVertex,
                prevEdge.Prev, nextEdge.Next);
            prevEdge.Prev!.Next = newEdge;
            nextEdge.Next!.Prev = newEdge;

            // Change of head and tail if needed
            bool deletingHead = prevEdge == Head || nextEdge == Head;
            bool deletingTail = prevEdge == Tail || nextEdge == Tail;
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
            else
                newEdge.Prev.ChangeVertexPos(newEdge.PrevVertex, newEdge.Prev.PrevVertex);

            if (newEdge.Next is not BezierEdge)
            {
                var newNextVertex = new Vertex(newEdge.NextVertex);
                newEdge.NextVertex = newNextVertex;
                newEdge.Next!.PrevVertex = newNextVertex;
            }
            else
                newEdge.Next.ChangeVertexPos(newEdge.NextVertex, newEdge.Next.NextVertex);

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
            else
                prevEdge.Prev.ChangeVertexPos(prevEdge.PrevVertex, prevEdge.Prev.PrevVertex);

            if (nextEdge.Next is not BezierEdge)
            {
                var newNextVertex = new Vertex(e.NextVertex);
                nextEdge.Next.PrevVertex = newNextVertex;
                nextEdge.NextVertex = newNextVertex;
            }
            else
                nextEdge.Next.ChangeVertexPos(nextEdge.NextVertex, nextEdge.Next.NextVertex);

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
                    ReplaceVertex(newEdge.PrevVertex, new ContinuityVertex(newEdge.PrevVertex));
                if (oldEdge.Next is not BezierEdge)
                    ReplaceVertex(newEdge.NextVertex, new ContinuityVertex(newEdge.NextVertex));
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
            (Edge? prevEdge, Edge? nextEdge) = GetAdjecentEdges(oldVertex);

            prevEdge!.NextVertex = newVertex;
            nextEdge!.PrevVertex = newVertex;
        }

        public (Edge? e1, Edge? e2) GetAdjecentEdges(Vertex v)
        {
            Edge? prevEdge = null, nextEdge = null;
            TraverseAllList((Edge e) =>
            {
                if (e.PrevVertex == v)
                    nextEdge = e;
                else if (e.NextVertex == v)
                    prevEdge = e;

                if (e is BezierEdge)
                {
                    var be = (BezierEdge)e;
                    if (be.PrevControlVertex == v ||
                        be.NextControlVertex == v)
                    {
                        prevEdge = be.Prev;
                        nextEdge = be.Next;
                    }
                }

                return prevEdge is not null && nextEdge is not null;
            });
            return (prevEdge, nextEdge);
        }

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
            TraverseAllList((Edge e) =>
            {
                e.NextVertex.IsLocked = false;
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
