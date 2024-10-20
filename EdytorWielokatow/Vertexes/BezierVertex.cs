using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdytorWielokatow.Vertexes
{
    public enum ContinuityClasses { C0, G0, C1 };

    public class BezierVertex : Vertex
    {

        public static new Icon? iconC0 =
            Icon.FromHandle(new Bitmap("Resources\\C0.png").GetHicon());
        public static new Icon? iconG0 =
            Icon.FromHandle(new Bitmap("Resources\\G0.png").GetHicon());
        public static new Icon? iconC1 =
            Icon.FromHandle(new Bitmap("Resources\\C1.png").GetHicon());
        public static readonly Rectangle rect = new Rectangle(-10, 10, 20, 20);

        public ContinuityClasses ContinuityClass { get; set; }

        public BezierVertex(int x, int y,
            ContinuityClasses continuityClass = ContinuityClasses.C0, bool isLocked = false)
            : base(x, y, isLocked)
        {
            ContinuityClass = continuityClass;
        }

        public BezierVertex(Vertex v, ContinuityClasses continuityClass = ContinuityClasses.C0)
            : this(v.X, v.Y, continuityClass, v.IsLocked)
        {
        }

        public override Icon? GetIcon()
            => ContinuityClass switch
            {
                ContinuityClasses.G0 => iconG0,
                ContinuityClasses.C1 => iconC1,
                _ => iconC0
            };

        public override Rectangle GetIconRectangle() => rect;
    }
}
