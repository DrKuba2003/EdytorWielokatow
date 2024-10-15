using System.CodeDom;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Windows.Forms.VisualStyles;
using EdytorWielokatow.Edges;
using EdytorWielokatow.Utils;
using EdytorWielokatow.Vertexes;

namespace EdytorWielokatow
{
    public partial class Form1 : Form
    {
        private enum AppStates { CreatingPoly, DraggingPoint, DraggingEdge, DraggingPoly, AdmiringPoly };

        private const int RADIUS = 8;
        private const int EDGE_BUFFER = 1;

        private AppStates appState;
        private Bitmap drawArea;

        private EdgesList edgesList;
        private Vertex? startingPt;
        private Vertex? selectedPoint;
        private Edge? selectedEdge;
        private Vertex? cursorOldPos;

        // TODO walidacja koncowa krawedz
        // TODO walidacja przy zamianie typu

        public Form1()
        {
            InitializeComponent();

            appState = AppStates.CreatingPoly;
            edgesList = new EdgesList();

            drawArea = new Bitmap(Canvas.Size.Width, Canvas.Size.Height);
            Canvas.Image = drawArea;
            Draw();

        }

        private void Canvas_MouseDown(object sender, MouseEventArgs e)
        {
            var ptClicked = new Vertex(e.X, e.Y);
            if (e.Button == MouseButtons.Left)
            {
                if (appState == AppStates.CreatingPoly)
                {
                    Edge? newEdge = null;

                    if (startingPt is null)
                    {
                        startingPt = ptClicked;
                    }
                    else
                    {
                        // checking if newVert is startingVert and triangle minimum
                        bool isClosingPoly = edgesList.Count >= 2 &&
                            GeometryUtils.CheckIf2PClose(startingPt!, ptClicked, RADIUS);

                        if (isClosingPoly)
                            ptClicked = startingPt!;

                        var lastVert = edgesList.Count == 0 ?
                            startingPt! :
                            edgesList.Tail!.NextVertex;

                        newEdge = new Edge(lastVert, ptClicked);
                        edgesList.AddEdgeAtEnd(newEdge);

                        // Powiazanie poczatka z koncem
                        if (isClosingPoly)
                        {
                            newEdge.Next = edgesList.Head;
                            edgesList.Head!.Prev = newEdge;
                            appState = AppStates.AdmiringPoly;
                        }
                    }

                    Draw();
                }
                else if (appState == AppStates.AdmiringPoly)
                {
                    (selectedPoint, selectedEdge) = GetClickedObject(ptClicked);
                    if (selectedPoint is not null)
                    {
                        appState = AppStates.DraggingPoint;
                    }
                    else if (selectedEdge is not null)
                    {
                        appState = AppStates.DraggingEdge;
                    }
                    else
                    {
                        var bb = edgesList.CalculateBoundingBox();
                        if (ptClicked.X >= bb.minX &&
                            ptClicked.X <= bb.maxX &&
                            ptClicked.Y >= bb.minY &&
                            ptClicked.Y <= bb.maxY)
                            appState = AppStates.DraggingPoly;
                    }
                    cursorOldPos = new Vertex(e.X, e.Y);
                }
            }
            else if (e.Button == MouseButtons.Right &&
                appState == AppStates.AdmiringPoly)
            {
                (var ptR, var edgeR) = GetClickedObject(ptClicked);
                if (ptR is not null)
                {
                    vertexContextMenu.Show(Canvas, ptClicked.X, ptClicked.Y);
                    selectedPoint = ptR;
                }
                else if (edgeR is not null)
                {
                    edgeContextMenu.Show(Canvas, ptClicked.X, ptClicked.Y);
                    selectedEdge = edgeR;
                }
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (appState == AppStates.CreatingPoly &&
                startingPt is not null)
            {
                cursorOldPos = new Vertex(e.X, e.Y);
                Draw();
            }
            else if (e.Button == MouseButtons.Left &&
                cursorOldPos is not null)
            {
                if (appState == AppStates.DraggingPoint &&
                    selectedPoint is not null)
                {
                    if (IsVertexOutsideCanvas(new Vertex(e.X, e.Y))) return;

                    selectedPoint.X = e.X;
                    selectedPoint.Y = e.Y;
                    selectedPoint.IsLocked = true;

                    (Edge? ePrev, Edge? eNext) = edgesList.GetAdjecentEdges(selectedPoint);
                    if (ePrev is null || eNext is null) return;

                    var queue = new Queue<(bool isPrev, Edge e, Vertex changed, Vertex changing)>();
                    queue.Enqueue((false, ePrev, ePrev.PrevVertex, ePrev.NextVertex));
                    queue.Enqueue((true, eNext, eNext.NextVertex, eNext.PrevVertex));

                    ValidateEdges(queue);

                    edgesList.UnlockAllVertexes();

                    Draw();
                }
                else if (appState == AppStates.DraggingEdge &&
                    selectedEdge is not null)
                {
                    Vertex vec = new Vertex(e.X - cursorOldPos.X,
                        e.Y - cursorOldPos.Y);

                    if (IsVertexOutsideCanvas(selectedEdge.PrevVertex + vec) ||
                        IsVertexOutsideCanvas(selectedEdge.NextVertex + vec)) return;

                    selectedEdge.PrevVertex.X += vec.X;
                    selectedEdge.PrevVertex.Y += vec.Y;
                    selectedEdge.PrevVertex.IsLocked = true;

                    selectedEdge.NextVertex.X += vec.X;
                    selectedEdge.NextVertex.Y += vec.Y;
                    selectedEdge.NextVertex.IsLocked = true;

                    var queue = new Queue<(bool isPrev, Edge e, Vertex changed, Vertex changing)>();
                    if (selectedEdge.Next is not null)
                        queue.Enqueue((false, selectedEdge.Next, selectedEdge.Next.PrevVertex, selectedEdge.Next.NextVertex));
                    if (selectedEdge.Prev is not null)
                        queue.Enqueue((true, selectedEdge.Prev, selectedEdge.Prev.NextVertex, selectedEdge.Prev.PrevVertex));

                    ValidateEdges(queue);

                    edgesList.UnlockAllVertexes();

                    Draw();
                }
                else if (appState == AppStates.DraggingPoly)
                {
                    Vertex vec = new Vertex(e.X - cursorOldPos.X,
                        e.Y - cursorOldPos.Y);

                    edgesList.MoveWholePolygon(vec);
                    Draw();
                }
            }
            cursorOldPos = new Vertex(e.X, e.Y);
        }

        private void Canvas_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (appState == AppStates.DraggingPoint ||
                    appState == AppStates.DraggingEdge ||
                    appState == AppStates.DraggingPoly)
                {
                    selectedPoint = null;
                    selectedEdge = null;
                    appState = AppStates.AdmiringPoly;
                    Draw(true);
                }
            }
        }

        private void ValidateEdges(Queue<(bool isPrev, Edge e, Vertex changed, Vertex changing)> queue)
        {
            while (queue.Count > 0)
            {
                var item = queue.Dequeue();
                item.changing.CopyData(item.e.ChangeVertexPos(item.changed, item.changing));
                item.changing.IsLocked = true;

                if (item.isPrev)
                {
                    if (!(item.e.Prev!.PrevVertex.IsLocked)
                        && item.e.Prev!.GetType() != typeof(Edge))
                    {
                        queue.Enqueue((true, item.e.Prev, item.e.Prev.NextVertex, item.e.Prev.PrevVertex));
                    }
                }
                else
                {
                    if (!(item.e.Next!.NextVertex.IsLocked)
                        && item.e.Next!.GetType() != typeof(Edge))
                    {
                        queue.Enqueue((false, item.e.Next, item.e.Next.PrevVertex, item.e.Next.NextVertex));
                    }
                }
            }
        }

        private (Vertex? pt, Edge? e) GetClickedObject(Vertex ptClicked)
        {
            Vertex? ptOut = null;
            double minPtDist = double.MaxValue;
            Edge? edgeOut = null;
            double minEdgeDist = double.MaxValue;

            if (appState == AppStates.CreatingPoly) return (ptOut, edgeOut);

            edgesList.TraverseAllList((Edge e) =>
            {
                double ptDist = GeometryUtils.DistB2P(ptClicked, e.NextVertex);
                if (ptDist < RADIUS && ptDist < minPtDist)
                {
                    minPtDist = ptDist;
                    ptOut = e.NextVertex;
                }

                if (ptOut is null)
                {
                    double edgeDist = GeometryUtils.DistBPE(ptClicked, e);
                    if (edgeDist < EDGE_BUFFER && edgeDist < minEdgeDist)
                    {
                        minEdgeDist = edgeDist;
                        edgeOut = e;
                    }
                }
                return false;
            });

            // If point found, return it not edge
            if (ptOut is not null)
                edgeOut = null;

            return (ptOut, edgeOut);
        }

        private void usunToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedPoint is null) return;

            if (edgesList.Count == 3)
                ResetPoly();
            else if (edgesList.DeleteVertex(selectedPoint))
                return;

            selectedPoint = null;
            Draw();
        }

        private void podpodzielToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedEdge is null) return;

            edgesList.EdgeSubdivison(selectedEdge);

            selectedEdge = null;
            Draw();
        }

        private void pionowaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedEdge is null ||
                selectedEdge.Prev!.GetType() == typeof(VerticalEdge) ||
                selectedEdge.Next!.GetType() == typeof(VerticalEdge)) return;

            edgesList.ReplaceEdge(selectedEdge, new VerticalEdge(selectedEdge));

            selectedEdge = null;
            Draw();
        }

        private void poziomaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedEdge is null ||
                selectedEdge.Prev!.GetType() == typeof(HorizontalEdge) ||
                selectedEdge.Next!.GetType() == typeof(HorizontalEdge)) return;

            edgesList.ReplaceEdge(selectedEdge, new HorizontalEdge(selectedEdge));

            selectedEdge = null;
            Draw();
        }

        private void stalaDlugoscToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedEdge is null) return;

            double L = GeometryUtils.DistB2P(selectedEdge.PrevVertex, selectedEdge.NextVertex);

            L = new FixedLengthDialog().Show(L);

            var newEdge = new FixedLengthEdge(selectedEdge, L);
            edgesList.ReplaceEdge(selectedEdge, newEdge);

            // TODO poprawic to
            var queue = new Queue<(bool isPrev, Edge e, Vertex changed, Vertex changing)>();
            if (newEdge.Next is not null)
                queue.Enqueue((false, newEdge.Next, newEdge.Next.PrevVertex, newEdge.Next.NextVertex));
            if (newEdge.Prev is not null)
                queue.Enqueue((true, newEdge.Prev, newEdge.Prev.NextVertex, newEdge.Prev.PrevVertex));

            ValidateEdges(queue);

            edgesList.UnlockAllVertexes();

            selectedEdge = null;
            Draw();
        }

        private void usunOgraniczeniaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedEdge is null) return;

            edgesList.ReplaceEdge(selectedEdge, new Edge(selectedEdge));

            Draw();
        }

        private void Draw(bool useBresenham = false)
        {
            using (Graphics g = Graphics.FromImage(drawArea))
            {
                g.Clear(Color.White);

                if (appState == AppStates.CreatingPoly && startingPt is not null)
                {
                    g.FillEllipse(Brushes.Blue,
                        startingPt.X - RADIUS, startingPt.Y - RADIUS,
                        2 * RADIUS, 2 * RADIUS);

                    if (cursorOldPos is not null)
                    {
                        var vStart = edgesList.Tail is null ? startingPt : edgesList.Tail.NextVertex;
                        g.DrawLine(new Pen(Brushes.Blue, 2),
                                vStart.X, vStart.Y,
                                cursorOldPos.X, cursorOldPos.Y);
                        cursorOldPos = null;
                    }
                }

                edgesList.TraverseAllList((Edge e) =>
                {
#if DEBUG
                    Brush b = e == edgesList.Head ?
                        Brushes.Green :
                        e == edgesList.Tail ?
                        Brushes.Red :
                        Brushes.Blue;
#else
                        Brush b = Brushes.Blue; 
#endif
                    if (useBresenham)
                        GeometryUtils.Bresenhams(g, e.PrevVertex.X, e.PrevVertex.Y,
                            e.NextVertex.X, e.NextVertex.Y, b);
                    else
                        g.DrawLine(new Pen(b, 3),
                                e.PrevVertex.X, e.PrevVertex.Y,
                                e.NextVertex.X, e.NextVertex.Y);

                    g.FillEllipse(Brushes.Blue,
                                e.NextVertex.X - RADIUS, e.NextVertex.Y - RADIUS,
                                2 * RADIUS, 2 * RADIUS);

                    var icon = e.GetIcon();
                    if (icon is not null)
                    {
                        var midpt = GeometryUtils.Midpoint(e.PrevVertex, e.NextVertex);
                        var rect = e.GetIconRectangle();
                        rect.Offset(new Point(midpt.X, midpt.Y));
                        g.DrawIcon(icon, rect);
                    }

                    return false;
                });

#if DEBUG
                g.DrawString($"Edge cout: {edgesList.Count.ToString()}",
                    SystemFonts.DefaultFont, Brushes.Black, new PointF(10, 10));
                g.DrawString($"App stat: {appState.ToString()}",
                    SystemFonts.DefaultFont, Brushes.Black, new PointF(10, 25));
#endif
            }
            Canvas.Refresh();
        }

        private bool IsVertexOutsideCanvas(Vertex v) =>
             (v.X < 0 || v.X > Canvas.Width ||
              v.Y < 0 || v.Y > Canvas.Height);

        private void ResetPoly()
        {
            edgesList.DeleteAll();
            startingPt = null;
            appState = AppStates.CreatingPoly;
        }

    }
}
