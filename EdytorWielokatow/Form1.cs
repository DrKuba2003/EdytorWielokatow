using System.CodeDom;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Windows.Forms.VisualStyles;
using EdytorWielokatow.Edges;
using EdytorWielokatow.Utils;

namespace EdytorWielokatow
{
    public partial class Form1 : Form
    {
        private enum AppStates { CreatingPoly, DraggingPoint, DraggingEdge, AdmiringPoly };

        private const int RADIUS = 8;
        private const int EDGE_BUFFER = 2;

        private AppStates appState;
        private Bitmap drawArea;

        private EdgesList edgesList;
        private Vertex? startingPt;
        private Vertex? selectedPoint;
        private Edge? selectedEdge;
        private Vertex? cursorOldPos;

        private Dictionary<Type, Icon> edgeBitmaps = new()
        {
            {typeof(FixedLengthEdge), Icon.FromHandle(new Bitmap("Resources\\FixedLength.png").GetHicon()) },
            {typeof(HorizontalEdge), Icon.FromHandle(new Bitmap("Resources\\Horizontal.png").GetHicon()) },
            {typeof(VerticalEdge), Icon.FromHandle(new Bitmap("Resources\\Vertical.png").GetHicon()) }
        };

        private Dictionary<Type, Rectangle> edgeBitmapRects = new()
        {
            {typeof(FixedLengthEdge), new Rectangle(-10, -10, 20, 20) },
            {typeof(HorizontalEdge), new Rectangle(-10, -5, 20, 20) },
            {typeof(VerticalEdge), new Rectangle(-5, -10, 20, 20) }
        };

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
                        cursorOldPos = new Vertex(e.X, e.Y);
                    }
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
            if (e.Button == MouseButtons.Left)
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
                    selectedEdge is not null &&
                    cursorOldPos is not null)
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

                    cursorOldPos = new Vertex(e.X, e.Y);
                    Draw();
                }
            }
        }

        private void Canvas_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (appState == AppStates.DraggingPoint)
                {

                    selectedPoint = null;
                    appState = AppStates.AdmiringPoly;
                    Draw(true);
                }
                else if (appState == AppStates.DraggingEdge)
                {

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

            edgesList.ReplaceEdge(selectedEdge, new FixedLengthEdge(selectedEdge));

            selectedEdge = null;
            Draw();
        }

        private void Draw(bool useBresenham = false)
        {
            using (Graphics g = Graphics.FromImage(drawArea))
            {
                g.Clear(Color.White);
                g.DrawEllipse(new Pen(Color.Red, 2),
                    Canvas.Size.Width / 2 - 2 * RADIUS,
                    Canvas.Size.Height / 2 - 2 * RADIUS,
                    4 * RADIUS, 4 * RADIUS);

                if (appState == AppStates.CreatingPoly && startingPt is not null)
                {
                    g.FillEllipse(Brushes.Blue,
                        startingPt.X - RADIUS, startingPt.Y - RADIUS,
                        2 * RADIUS, 2 * RADIUS);
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

                    if (e.GetType() != typeof(Edge))
                    {
                        var midpt = GeometryUtils.Midpoint(e.PrevVertex, e.NextVertex);
                        var rect = edgeBitmapRects[e.GetType()];
                        rect.Offset(new Point(midpt.X, midpt.Y));
                        var icon = edgeBitmaps[e.GetType()];
                        g.DrawIcon(icon, rect);
                    }

                    return false;
                });

#if DEBUG
                g.DrawString($"Edge cout: {edgesList.Count.ToString()}",
                    SystemFonts.DefaultFont, Brushes.Black, new PointF(10, 10));
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
