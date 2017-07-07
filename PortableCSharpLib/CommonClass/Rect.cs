namespace PortableCSharpLib.CommonClass
{
    /// <summary>
    /// Basic class for rectangle, with origin at top left. X axis pointing right, and Yaxis pointing down.
    /// </summary>
    public class Rect
    {
        public Rect()
        {
            Top = Left = Bottom = Right = -1;
        }
        public Rect(int t, int l, int b, int r)
        {
            Top = t;
            Left = l;
            Bottom = b;
            Right = r;
        }
        //public Rect(int t, int l, int w, int h)
        //{
        //    Top = t;
        //    Left = l;
        //    Bottom = Top + h - 1;
        //    Right = Left + w - 1;
        //}
        public int Width { get { return Right - Left + 1; } }
        public int Height { get { return Bottom - Top + 1; } }
        public int Left { get; set; }
        public int Top { get; set; }
        public int Right { get; set; }
        public int Bottom { get; set; }
    }
}
