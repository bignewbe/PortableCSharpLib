using PortableCSharpLib.DataType;
using System;
using System.Drawing;

namespace PortableCSharpLib.DataType
{
    [Serializable]
    public class CRect2
    {
        static CRect2() { PortableCSharpLib.General.CheckDateTime(); }

        public Rectangle Rect { get { return new Rectangle(Left, Top, Width, Height); } }

        public int Top { get; set; }
        public int Left { get; set; }
        public int Bottom { get; set; }
        public int Right { get; set; }
        public int Width { get { return Right - Left + 1; } }
        public int Height { get { return Bottom - Top + 1; } }

        public string _label;
        public bool _selected;
        public int _threshold;
        public bool _isBackgroundDark;
        public char _character;
        public int _numPixelCharacter;
        public int[] _dist;

        public override string ToString()
        {
            return _label + ": " + Rect.ToString();
            //return string.Format("Top = {0}, Left = {1}, W = {2}, Right = {3}", Top, Left, Bottom, Right);
        }
        public CRect2()
        {
            _character = 'x';
            _numPixelCharacter = 0;
            //_rect = new Rectangle();
            _dist = new int[4] { 0, 0, 0, 0 };
            _threshold = -1;
            _label = string.Empty;
            _selected = false;
        }
        public CRect2(int top, int left, int bottom, int right)
            : this()
        {
            Top = top;
            Left = left;
            Bottom = bottom;
            Right = right;
        }

        public CRect2(CRect2 r)
            : this(r.Top, r.Left, r.Bottom, r.Right)
        {
        }

        public CRect2(Rectangle r)
            : this(r.Top, r.Left, r.Bottom, r.Right)
        {
        }

        public CRect2(Rect r)
            : this(r.Top, r.Left, r.Bottom, r.Right)
        {
        }

        public void ShiftRect(int dx, int dy)
        {
            Left += dx;
            Right += dx;
            Top += dy;
            Bottom += dy;
        }
    }
}
