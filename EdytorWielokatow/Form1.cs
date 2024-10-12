using System.Drawing;
using System.Windows.Forms.VisualStyles;
using EdytorWielokatow.Edges;
using EdytorWielokatow.Utils;

namespace EdytorWielokatow
{
    public partial class Form1 : Form
    {
        private enum AppStates { CreatingPoly, DraggingPoint, DraggingEdge, AdmiringPoly };

        private const int RADIUS = 8;
        private const int BUFFER = RADIUS + 5;

        private AppStates appState;
        private Bitmap drawArea;
        private List<Edge> edges;

        private Vertex? startingPt;
        private Vertex? selectedPoint;
        private Edge? selectedEdge;

        public Form1()
        {
            InitializeComponent();

            appState = AppStates.CreatingPoly;
            edges = new List<Edge>();

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
                        if (GeometryUtils.CheckIf2PClose(startingPt!, ptClicked, BUFFER))
                        {
                            // tutaj to sprawdzenie, zeby nie kliknac trzech punktow
                            // bardzo blisko siebie
                            if (edges.Count < 2) return;

                            ptClicked = startingPt!;
                            appState = AppStates.AdmiringPoly;
                        }

                        // Creating edge
                        var lastVert = edges.Count == 0 ?
                            startingPt! :
                            edges.Last().End;
                        newEdge = new Edge(lastVert, ptClicked);
                        edges.Add(newEdge);
                    }

                    // Drawing
                    using (Graphics g = Graphics.FromImage(drawArea))
                    {
                        g.FillEllipse(Brushes.Blue,
                            ptClicked.X - RADIUS, ptClicked.Y - RADIUS,
                            2 * RADIUS, 2 * RADIUS);

                        if (newEdge is not null)
                            g.DrawLine(new Pen(Brushes.Blue, 3),
                                newEdge.Start.X, newEdge.Start.Y,
                                newEdge.End.X, newEdge.End.Y);
                    }

                    Canvas.Refresh();
                }
                else if (appState == AppStates.AdmiringPoly)
                {
                    (var ptR, var edgeR) = GetClickedObject(ptClicked);
                    if (ptR is not null)
                    {
                        selectedPoint = ptR;
                        appState = AppStates.DraggingPoint;
                    }
                    else if (edgeR is not null)
                    {
                        Edge edge = (Edge)edgeR;
                        using (Graphics g = Graphics.FromImage(drawArea))
                        {
                            g.DrawLine(new Pen(Brushes.GreenYellow, 3),
                                edge.Start.X, edge.Start.Y,
                                edge.End.X, edge.End.Y);
                        }
                        Canvas.Refresh();
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
                    if (e.X < 0 || e.X > Canvas.Width ||
                        e.Y < 0 || e.Y > Canvas.Height) return;

                    selectedPoint.X = e.X;
                    selectedPoint.Y = e.Y;

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
                    appState = AppStates.AdmiringPoly;
                    selectedPoint = null;
                    Draw();
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

            foreach (var e in edges)
            {
                double ptDist = GeometryUtils.SquaredDistB2P(ptClicked, e.End);
                if (ptDist < Math.Pow(BUFFER, 2) && ptDist < minPtDist)
                {
                    minPtDist = ptDist;
                    ptOut = e.End;
                }

                if (ptOut is null)
                {
                    double edgeDist = GeometryUtils.DistBPE(ptClicked, e);
                    if (edgeDist < BUFFER && edgeDist < minEdgeDist)
                    {
                        minEdgeDist = edgeDist;
                        edgeOut = e;
                    }
                }
            }

            // If point found, return it not edge
            if (ptOut is not null)
                edgeOut = null;

            return (ptOut, edgeOut);
        }

        private void Draw()
        {
            using (Graphics g = Graphics.FromImage(drawArea))
            {
                g.Clear(Color.White);
                g.DrawEllipse(new Pen(Color.Red, 2),
                    Canvas.Size.Width / 2 - 2 * RADIUS,
                    Canvas.Size.Height / 2 - 2 * RADIUS,
                    4 * RADIUS, 4 * RADIUS);

                foreach (var e in edges)
                {
                    g.DrawLine(new Pen(Brushes.Blue, 3),
                        e.Start.X, e.Start.Y,
                        e.End.X, e.End.Y);

                    g.FillEllipse(Brushes.Blue,
                                e.End.X - RADIUS, e.End.Y - RADIUS,
                                2 * RADIUS, 2 * RADIUS);
                }
            }
            Canvas.Refresh();
        }

        private void usunToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedPoint is not null)
            {
                selectedPoint.X = selectedPoint.X + 100;
                selectedPoint.Y = selectedPoint.Y + 100;
                selectedPoint = null;
                Draw();
            }
            
        }
    }
}
