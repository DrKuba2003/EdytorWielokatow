using System.CodeDom;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading.Channels;
using System.Windows.Forms.VisualStyles;
using EdytorWielokatow.Edges;
using EdytorWielokatow.Utils;
using EdytorWielokatow.Vertexes;

namespace EdytorWielokatow
{
    public partial class Form1 : Form
    {
        private enum AppStates { CreatingPoly, DraggingPoint, DraggingEdge, DraggingPoly, AdmiringPoly };

        private const int VERTEX_BUFFER = 8;
        private const int EDGE_BUFFER = 1;

        private AppStates appState;
        private Bitmap drawArea;

        private EdgesList edgesList;
        private Vertex? startingPt;
        private Vertex? selectedPoint;
        private Edge? selectedEdge;
        private Vertex? cursorOldPos;

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
                            GeometryUtils.CheckIf2PClose(startingPt!, ptClicked, VERTEX_BUFFER);

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

                    (Edge? prevEdge, Edge? nextEdge) = edgesList.GetAdjecentEdges(selectedPoint);
                    if (prevEdge is not null && nextEdge is not null)
                    {
                        if (ValidateEdges(prevEdge!, nextEdge!))
                        {
                            Vertex vec = new Vertex(e.X - cursorOldPos.X,
                                e.Y - cursorOldPos.Y);
                            edgesList.MoveWholePolygon(vec, new List<Vertex>() { selectedPoint });
                        }
                    }




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

                    selectedEdge.NextVertex.X += vec.X;
                    selectedEdge.NextVertex.Y += vec.Y;

                    if (ValidateEdges(selectedEdge.Prev!, selectedEdge.Next!))
                    {
                        edgesList.MoveWholePolygon(vec,
                            new List<Vertex>() { selectedEdge.PrevVertex, selectedEdge.NextVertex });
                    }

                    Draw();
                }
                else if (appState == AppStates.DraggingPoly)
                {
                    Vertex vec = new Vertex(e.X - cursorOldPos.X,
                        e.Y - cursorOldPos.Y);

                    edgesList.MoveWholePolygon(vec);
                    Draw();
                }
                cursorOldPos = new Vertex(e.X, e.Y);
            }
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

        private bool ValidateEdges(Edge prevEdge, Edge nextEdge)
        {
            (Edge edge, bool isPrev) last = (nextEdge, false);
            var roolback = new Dictionary<Vertex, Point>();
            var queue = new Queue<(bool isPrev, Edge e)>();

            prevEdge.NextVertex.IsLocked = true;
            nextEdge.PrevVertex.IsLocked = true;

            if (nextEdge.GetType() != typeof(Edge))
            {
                queue.Enqueue((false, nextEdge));
                nextEdge.NextVertex.IsLocked = true;
            }
            if (prevEdge.GetType() != typeof(Edge))
            {
                queue.Enqueue((true, prevEdge));
                prevEdge.PrevVertex.IsLocked = true;
            }

            while (queue.Count > 0)
            {
                var item = queue.Dequeue();

                var changed = item.isPrev ? item.e.NextVertex : item.e.PrevVertex;
                var changing = item.isPrev ? item.e.PrevVertex : item.e.NextVertex;

                roolback[changing] = new Point(changing.X, changing.Y);
                item.e.ChangeVertexPos(changed, changing);

                // TODO usunac
                //using (Graphics g = Graphics.FromImage(drawArea))
                //{
                //    g.DrawLine(new Pen(Brushes.YellowGreen, 3),
                //            item.e.PrevVertex.X, item.e.PrevVertex.Y,
                //            item.e.NextVertex.X, item.e.NextVertex.Y);

                //    g.FillEllipse(Brushes.Magenta,
                //             roolback[changing].X - RADIUS, roolback[changing].Y - RADIUS,
                //            2 * RADIUS, 2 * RADIUS);
                //}
                //Canvas.Refresh();

                if (item.isPrev)
                {
                    if (!item.e.Prev!.PrevVertex.IsLocked &&
                        item.e.Prev!.GetType() != typeof(Edge))
                    {
                        queue.Enqueue((true, item.e.Prev));
                        item.e.Prev.PrevVertex.IsLocked = true; // zeby bylo oznaczone ze bedzie zmienianie
                    }
                }
                else
                {
                    if (!item.e.Next!.NextVertex.IsLocked &&
                        item.e.Next!.GetType() != typeof(Edge))
                    {
                        queue.Enqueue((false, item.e.Next));
                        item.e.Next.NextVertex.IsLocked = true;
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


            edgesList.UnlockAllVertexes();

            return false;
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
                foreach (var pt in e.GetVertexesExceptPrev())
                {
                    double ptDist = GeometryUtils.DistB2P(ptClicked, pt);
                    if (ptDist < VERTEX_BUFFER && ptDist < minPtDist)
                    {
                        minPtDist = ptDist;
                        ptOut = pt;
                    }
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
                selectedEdge.Next!.GetType() == typeof(VerticalEdge))
            {
                ShowEdgeTypeError();
                selectedEdge = null;
                return;
            }

            var newEdge = new VerticalEdge(selectedEdge);

            if (ValidateEdges(newEdge.Prev!, newEdge.Next!))
                ShowEdgeTypeError();
            else
            {
                edgesList.ReplaceEdge(selectedEdge, newEdge);
                Draw();
            }

            selectedEdge = null;
        }

        private void poziomaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedEdge is null ||
                selectedEdge.Prev!.GetType() == typeof(HorizontalEdge) ||
                selectedEdge.Next!.GetType() == typeof(HorizontalEdge))
            {
                ShowEdgeTypeError();
                selectedEdge = null;
                return;
            }

            var newEdge = new HorizontalEdge(selectedEdge);

            if (ValidateEdges(newEdge.Prev!, newEdge.Next!))
                ShowEdgeTypeError();
            else
            {
                edgesList.ReplaceEdge(selectedEdge, newEdge);
                Draw();
            }

            selectedEdge = null;
        }

        private void stalaDlugoscToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedEdge is null) return;

            int L = (int)GeometryUtils.DistB2P(selectedEdge.PrevVertex, selectedEdge.NextVertex);

            L = new FixedLengthDialog().Show(L);

            var newEdge = new FixedLengthEdge(selectedEdge, L);

            if (ValidateEdges(newEdge.Prev!, newEdge.Next!))
                ShowEdgeTypeError();
            else
            {
                edgesList.ReplaceEdge(selectedEdge, newEdge);
                Draw();
            }

            selectedEdge = null;
        }

        private void bezToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedEdge is null) return;

            var midpoint = GeometryUtils.Midpoint(selectedEdge.PrevVertex, selectedEdge.NextVertex);
            var newEdge = new BezierEdge(selectedEdge,
                 new Vertex(midpoint.X, midpoint.Y + 50),
                 new Vertex(midpoint.X, midpoint.Y - 50));

            if (ValidateEdges(newEdge.Prev!, newEdge.Next!))
                ShowEdgeTypeError();
            else
            {
                edgesList.ReplaceEdge(selectedEdge, newEdge);
                edgesList.ReplaceVertex(newEdge.PrevVertex, new BezierVertex(newEdge.PrevVertex));
                edgesList.ReplaceVertex(newEdge.NextVertex, new BezierVertex(newEdge.NextVertex));
                Draw();
            }

            selectedEdge = null;
        }

        private void usunOgraniczeniaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedEdge is null) return;

            edgesList.ReplaceEdge(selectedEdge, new Edge(selectedEdge));

            Draw();
        }

        private void c0ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedPoint is null) return;

        }

        private void g0ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void g1ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void Draw(bool useBresenham = false)
        {
            using (Graphics g = Graphics.FromImage(drawArea))
            {
                g.Clear(Color.White);

                if (appState == AppStates.CreatingPoly && startingPt is not null)
                {
                    startingPt.Draw(g);

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


                    var midpt1 = GeometryUtils.Midpoint(e.PrevVertex, e.NextVertex);
                    g.DrawString(
                            $"{e.GetHashCode()}",
                            SystemFonts.DefaultFont, Brushes.Black, new PointF(midpt1.X - 20, midpt1.Y - 20)
                            );
#else
                        Brush b = Brushes.Blue; 
#endif

                    e.Draw(g, useBresenham, new Pen(b, 3));

                    e.NextVertex.Draw(g);

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

        private void ShowEdgeTypeError()
            => MessageBox.Show("Nie mo¿na zmieniæ typu krawêdzi!!!", "B³¹d",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);

        private void ResetPoly()
        {
            edgesList.DeleteAll();
            startingPt = null;
            appState = AppStates.CreatingPoly;
        }
    }
}
