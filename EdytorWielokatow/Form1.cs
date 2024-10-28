using EdytorWielokatow.Edges;
using EdytorWielokatow.Utils;
using EdytorWielokatow.Vertexes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EdytorWielokatow
{
    public partial class Form1 : Form
    {
        private enum AppStates { CreatingPolygon, DraggingPoint, DraggingEdge, DraggingPolygon, LookingAtPolygon };

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

            appState = AppStates.CreatingPolygon;
            edgesList = new EdgesList();

            drawArea = new Bitmap(Canvas.Size.Width, Canvas.Size.Height);
            Canvas.Image = drawArea;

            CreateStartupPolygon();

            Draw();

        }

        private void Canvas_MouseDown(object sender, MouseEventArgs e)
        {
            var ptClicked = new Vertex(e.X, e.Y);
            if (e.Button == MouseButtons.Left)
            {
                if (appState == AppStates.CreatingPolygon)
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
                            appState = AppStates.LookingAtPolygon;
                        }
                    }

                    Draw();
                }
                else if (appState == AppStates.LookingAtPolygon)
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
                            appState = AppStates.DraggingPolygon;
                    }
                    cursorOldPos = new Vertex(e.X, e.Y);
                }
            }
            else if (e.Button == MouseButtons.Right )
            {
                (selectedPoint, selectedEdge) = GetClickedObject(ptClicked);
                if (selectedPoint is not null && selectedPoint is not ControlVertex &&
                appState == AppStates.LookingAtPolygon)
                    vertexContextMenu.Show(Canvas, ptClicked.X.Round(), ptClicked.Y.Round());
                else if (selectedEdge is not null && appState == AppStates.LookingAtPolygon)
                    edgeContextMenu.Show(Canvas, ptClicked.X.Round(), ptClicked.Y.Round());
                else
                    generalContexMenu.Show(Canvas, ptClicked.X.Round(), ptClicked.Y.Round());
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (appState == AppStates.CreatingPolygon &&
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
                    selectedPoint.X = e.X;
                    selectedPoint.Y = e.Y;

                    var rollback = new Dictionary<Vertex, PointF>();
                    if (selectedPoint is ControlVertex)
                    {
                        ControlVertex cv = (ControlVertex)selectedPoint;
                        if (cv.Edge.PrevControlVertex == cv)
                            rollback[cv.Edge.PrevVertex] = new PointF(cv.Edge.PrevVertex.X,
                                cv.Edge.PrevVertex.Y);
                        else
                            rollback[cv.Edge.NextVertex] = new PointF(cv.Edge.NextVertex.X,
                                cv.Edge.NextVertex.Y);

                        cv.Edge.ControlChangeVertexPos(cv);
                    }

                    (Edge? prevEdge, Edge? nextEdge) = edgesList.GetAdjecentEdges(selectedPoint);
                    if (prevEdge is not null && nextEdge is not null)
                    {
                        if (edgesList.ValidateEdges(prevEdge!, nextEdge!, rollback))
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

                    selectedEdge.PrevVertex.X += vec.X;
                    selectedEdge.PrevVertex.Y += vec.Y;

                    foreach (var v in selectedEdge.GetVertexesExceptPrev())
                    {
                        v.X += vec.X;
                        v.Y += vec.Y;
                    }

                    if (edgesList.ValidateEdges(selectedEdge.Prev!, selectedEdge.Next!))
                    {
                        edgesList.MoveWholePolygon(vec,
                            new List<Vertex>() { selectedEdge.PrevVertex, selectedEdge.NextVertex });
                    }

                    Draw();
                }
                else if (appState == AppStates.DraggingPolygon)
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
                    appState == AppStates.DraggingPolygon)
                {
                    selectedPoint = null;
                    selectedEdge = null;
                    appState = AppStates.LookingAtPolygon;
                    Draw(true);
                }
            }
        }

        private (Vertex? pt, Edge? e) GetClickedObject(Vertex ptClicked)
        {
            Vertex? ptOut = null;
            double minPtDist = double.MaxValue;
            Edge? edgeOut = null;
            double minEdgeDist = double.MaxValue;

            if (appState == AppStates.CreatingPolygon) return (ptOut, edgeOut);

            edgesList.TraverseAllList((Edge e) =>
            {
                foreach (var v in e.GetVertexesExceptPrev())
                {
                    double ptDist = GeometryUtils.DistB2P(ptClicked, v);
                    if (ptDist < VERTEX_BUFFER && ptDist < minPtDist)
                    {
                        minPtDist = ptDist;
                        ptOut = v;
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

            Vertex rollbackPrev = new Vertex(selectedEdge.PrevVertex);
            Vertex rollbackNext = new Vertex(selectedEdge.NextVertex);
            var newEdge = new VerticalEdge(selectedEdge);

            if (edgesList.ValidateEdges(newEdge.Prev!, newEdge.Next!))
            {
                selectedEdge.PrevVertex.CopyData(rollbackPrev);
                selectedEdge.NextVertex.CopyData(rollbackNext);

                ShowEdgeTypeError();
            }
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

            Vertex rollbackPrev = new Vertex(selectedEdge.PrevVertex);
            Vertex rollbackNext = new Vertex(selectedEdge.NextVertex);
            var newEdge = new HorizontalEdge(selectedEdge);

            if (edgesList.ValidateEdges(newEdge.Prev!, newEdge.Next!))
            {
                selectedEdge.PrevVertex.CopyData(rollbackPrev);
                selectedEdge.NextVertex.CopyData(rollbackNext);

                ShowEdgeTypeError();
            }
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

            Vertex rollbackPrev = new Vertex(selectedEdge.PrevVertex);
            Vertex rollbackNext = new Vertex(selectedEdge.NextVertex);
            var newEdge = new FixedLengthEdge(selectedEdge, L);

            if (edgesList.ValidateEdges(newEdge.Prev!, newEdge.Next!) || L == 0 || L > 10000)
            {
                selectedEdge.PrevVertex.CopyData(rollbackPrev);
                selectedEdge.NextVertex.CopyData(rollbackNext);

                ShowEdgeTypeError();
            }
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
            var newEdge = new BezierEdge(selectedEdge);

            edgesList.ReplaceEdge(selectedEdge, newEdge);
            edgesList.ValidateEdges(newEdge, newEdge);
            Draw();


            selectedEdge = null;
        }

        private void usunOgraniczeniaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedEdge is null) return;

            edgesList.ReplaceEdge(selectedEdge, new Edge(selectedEdge));

            Draw();
        }

        private void g0ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedPoint is null) return;

            if (selectedPoint is ContinuityVertex)
            {
                ContinuityVertex v = (ContinuityVertex)selectedPoint;
                v.ContinuityClass = ContinuityClasses.G0;

                Draw();
            }
        }

        private void g1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedPoint is null) return;

            if (selectedPoint is ContinuityVertex)
            {
                ContinuityVertex v = (ContinuityVertex)selectedPoint;
                v.ContinuityClass = ContinuityClasses.G1;

                (Edge? prevEdge, Edge? nextEdge) = edgesList.GetAdjecentEdges(selectedPoint);
                if (nextEdge is BezierEdge)
                    nextEdge.ChangeVertexPos(selectedPoint, nextEdge.NextVertex);

                if (prevEdge is BezierEdge)
                    prevEdge.ChangeVertexPos(selectedPoint, prevEdge.PrevVertex);

                Draw();
            }
        }

        private void c1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedPoint is null) return;

            if (selectedPoint is ContinuityVertex)
            {
                ContinuityVertex v = (ContinuityVertex)selectedPoint;
                v.ContinuityClass = ContinuityClasses.C1;

                (Edge? prevEdge, Edge? nextEdge) = edgesList.GetAdjecentEdges(selectedPoint);
                if (nextEdge is BezierEdge)
                    nextEdge.ChangeVertexPos(selectedPoint, nextEdge.NextVertex);

                if (prevEdge is BezierEdge)
                    prevEdge.ChangeVertexPos(selectedPoint, prevEdge.PrevVertex);

                Draw();
            }
        }

        private void Draw(bool useBresenham = false)
        {
            using (Graphics g = Graphics.FromImage(drawArea))
            {
                g.Clear(Color.White);

                if (appState == AppStates.CreatingPolygon && startingPt is not null)
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
                    Brush b = Brushes.Blue;

                    e.Draw(g, useBresenham, new Pen(b, 3));

                    e.NextVertex.Draw(g);

                    return false;
                });

                g.DrawString($"Stan aplikacji: {appState.ToString()}",
                    SystemFonts.DefaultFont, Brushes.Black, new PointF(10, 10));

            }
            Canvas.Refresh();
        }

        private void ShowEdgeTypeError()
            => MessageBox.Show("Nie mo¿na zmieniæ typu krawêdzi!!!", "B³¹d",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);

        private void CreateStartupPolygon()
        {
            var v1 = new ContinuityVertex(450, 300);
            var v2 = new ContinuityVertex(950, 300);
            v2.ContinuityClass = ContinuityClasses.C1;
            var v3 = new Vertex(950, 500);
            var v4 = new Vertex(450, 500);

            var c1 = new Vertex(450, 150);
            var c2 = new Vertex(950, 200);

            var e1 = new BezierEdge(v1, v2, c1, c2);
            var e2 = new Edge(v2, v3);
            var e3 = new Edge(v3, v4);
            var e4 = new VerticalEdge(v4, v1);


            edgesList.AddEdgeAtEnd(e1);
            edgesList.AddEdgeAtEnd(e2);
            edgesList.AddEdgeAtEnd(e3);
            edgesList.AddEdgeAtEnd(e4);

            e4.Next = edgesList.Head;
            edgesList.Head!.Prev = e4;

            appState = AppStates.LookingAtPolygon;
        }

        private void usunCalyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResetPoly();
            Draw();
        }

        private void ResetPoly()
        {
            edgesList.DeleteAll();
            startingPt = null;
            appState = AppStates.CreatingPolygon;
        }

        private void Canvas_Click(object sender, EventArgs e)
        {

        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            edgesList.Save();
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            edgesList.Load();
            Draw();
        }
    }
}
