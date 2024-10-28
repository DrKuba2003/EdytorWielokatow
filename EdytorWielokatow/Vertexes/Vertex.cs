using EdytorWielokatow.Utils;
using System.Numerics;
using System.Text.Json.Serialization;

namespace EdytorWielokatow.Vertexes
{
    public class Vertex : SerializableClass
    {
        public new const string ClassName = "VERTEX";
        private const int RADIUS = 8;
        public static readonly Rectangle rect = new Rectangle(0, 0, 0, 0);

        public float X { get; set; }
        public float Y { get; set; }
        public bool IsLocked { get; set; }

        [JsonIgnore]
        public Vector2 Vector2 { get => new Vector2(X, Y); }

        [JsonConstructor]
        public Vertex(float X, float Y, bool IsLocked = false)
        {
            this.X = X;
            this.Y = Y;
            this.IsLocked = IsLocked;
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
        public void CopyData(PointF v)
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
                rect.Offset(new Point(X.Round(), Y.Round()));
                g.DrawIcon(icon, rect);
                icon.Dispose();
            }
        }

        public virtual Icon? GetIcon() => null;
        public virtual Rectangle GetIconRectangle() => rect;


    }

}
