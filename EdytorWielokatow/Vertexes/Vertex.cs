using EdytorWielokatow.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace EdytorWielokatow.Vertexes
{
    public class Vertex
    {
        private const int RADIUS = 8;
        public static Icon? icon { get => null; }
        public static readonly Rectangle rect = new Rectangle(0, 0, 0, 0);

        public int X { get; set; }
        public int Y { get; set; }
        public bool IsLocked { get; set; }

        public Vector2 Vector2 { get => new Vector2(X, Y); }

        public Vertex(int x, int y, bool isLocked = false)
        {
            X = x;
            Y = y;
            IsLocked = isLocked;
        }

        public Vertex(Vertex v)
        {
            X = v.X;
            Y = v.Y;
            IsLocked = v.IsLocked;
        }

        public void CopyData(Vertex v)
        {
            X = v.X;
            Y = v.Y;
            IsLocked = v.IsLocked;
        }
        public void CopyData(Point v)
        {
            X = v.X;
            Y = v.Y;
            IsLocked = false;
        }
        public virtual void Draw(Graphics g, Brush? b = null, int radius = RADIUS)
        {
            g.FillEllipse(b is null ? Brushes.Blue : b,
                    X - radius, Y - radius,
                    2 * radius, 2 * radius);

            var icon = GetIcon();
            if (icon is not null)
            {
                var rect = GetIconRectangle();
                rect.Offset(new Point(X, Y));
                g.DrawIcon(icon, rect);
                icon.Dispose();
            }
        }

        public virtual Icon? GetIcon() => icon;
        public virtual Rectangle GetIconRectangle() => rect;


    }

}
