namespace EdytorWielokatow.Vertexes
{
    public enum ContinuityClasses { G0, G1, C1 };

    public class ContinuityVertex : Vertex
    {
        public new const string ClassName = "CONTINUITYVERTEX";

        public static readonly Icon? iconG0 =
            Icon.FromHandle(new Bitmap("Resources\\G0.png").GetHicon());
        public static readonly Icon? iconG1 =
            Icon.FromHandle(new Bitmap("Resources\\G1.png").GetHicon());
        public static readonly Icon? iconC1 =
            Icon.FromHandle(new Bitmap("Resources\\C1.png").GetHicon());
        public static new readonly Rectangle rect = new Rectangle(-8, 10, 20, 20);

        public ContinuityClasses ContinuityClass { get; set; }

        public ContinuityVertex(float x, float y,
            ContinuityClasses continuityClass = ContinuityClasses.G1, bool isLocked = false)
            : base(x, y, isLocked)
        {
            ContinuityClass = continuityClass;
        }

        public ContinuityVertex(Vertex v, ContinuityClasses continuityClass = ContinuityClasses.G1)
            : this(v.X, v.Y, continuityClass, v.IsLocked)
        {
        }

        public override Icon? GetIcon()
            => ContinuityClass switch
            {
                ContinuityClasses.G1 => iconG1,
                ContinuityClasses.C1 => iconC1,
                _ => iconG0
            };

        public override Rectangle GetIconRectangle() => rect;
    }
}
