using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.CodeDom;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using Tesseract;
using System.Globalization;

namespace BananaHackV2.UI
{
    [TypeConverterAttribute(typeof(RectangleConverter))]
    [Serializable]
    internal struct RadialRect
    {
        private int _x;
        public int X
        {
            get {
                return _x;
            }
            set {
                _x = value;
            }
        }


        private int _y;
        public int Y
        {
            get {
                return _y;
            }
            set {
                _y = value;
            }
        }


        private int _width;
        public int Width
        {
            get {
                return _width;
            }
            set {
                _width = value;
            }
        }


        private int _height;
        public int Height
        {
            get {
                return _height;
            }
            set {
                _height = value;
            }
        }


        public Point Location
        {
            get {
                return new Point(X, Y);
            }
            set {                
                X = value.X;
                Y = value.Y;
            }
        }


        public Size Size
        {
            get {
                return new Size(Width, Height);
            }
            set {                
                _width = value.Width;
                _height = value.Height;
            }
        }


        public Point ZeroPoint
        {
            get {
                return new Point(
                    X + Width / 2, 
                    Y + Height / 2);
            }
        }

        public int XZero 
        { 
            get { 
                return X + Width / 2; 
            } 
        }

        public int YZero 
        { 
            get { 
                return Y + Height / 2; 
            } 
        }

        public int Left
        {
            get {
                return X;
            }
        }

        public int Top
        {
            get {
                return Y;
            }
        }

        public int Right
        {
            get {
                return X + Width;
            }
        }

        public int Bottom
        {
            get {
                return Y + Height;
            }
        }

        public bool IsEmpty
        {
            get {
                return 
                    _height == 0 && 
                    _width == 0 && 
                    X == 0 && 
                    Y == 0;
            }
        }


        public static implicit operator Rectangle(RadialRect rc)
        {
            return new Rectangle(
                rc.X, rc.Y, 
                rc.Width, rc.Height);
        }


        public static implicit operator RadialRect(Rectangle rc)
        {
            return new RadialRect(
                rc.X, rc.Y, 
                rc.Width, rc.Height);
        }


        public static bool operator == (RadialRect left, RadialRect right)
        {
            return (
                left.X == right.X && 
                left.Y == right.Y && 
                left.Width == right.Width && 
                left.Height == right.Height);
        }


        public static bool operator != (RadialRect left, RadialRect right)
        {
            return !(left == right);
        }


        public static RadialRect Ceiling(RectangleF value)
        {
            return new RadialRect(
                (int)Math.Ceiling(value.X),
                (int)Math.Ceiling(value.Y),
                (int)Math.Ceiling(value.Width),
                (int)Math.Ceiling(value.Height));
        }

        public static RadialRect Truncate(RectangleF value)
        {
            return new RadialRect(
                (int)value.X,
                (int)value.Y,
                (int)value.Width,
                (int)value.Height);
        }

        public static RadialRect Round(RectangleF value)
        {
            return new RadialRect(
                (int)Math.Round(value.X),
                (int)Math.Round(value.Y),
                (int)Math.Round(value.Width),
                (int)Math.Round(value.Height));
        }

        [Pure] 
        public bool Contains(int x, int y)
        {
            return this.X <= x && x < this.X + this.Width && 
                this.Y <= y && y < this.Y + this.Height;
        }

        [Pure]
        public bool Contains(Point pt)
        {
            return Contains(pt.X, pt.Y);
        }

        [Pure]
        public bool Contains(Rectangle rc)
        {
            return (this.X <= rc.X) &&
            ((rc.X + rc.Width) <= (this.X + this.Width)) &&
            (this.Y <= rc.Y) &&
            ((rc.Y + rc.Height) <= (this.Y + this.Height));
        }

        [Pure]
        public bool Contains(RadialRect rc)
        {
            return (this.X <= rc.X) &&
            ((rc.X + rc.Width) <= (this.X + this.Width)) &&
            (this.Y <= rc.Y) &&
            ((rc.Y + rc.Height) <= (this.Y + this.Height));
        }


        public void Inflate(int width, int height)
        {
            this.X -= width;
            this.Y -= height;
            this.Width += 2 * width;
            this.Height += 2 * height;
        }

        public void Inflate(Size size)
        {
            Inflate(size.Width, size.Height);
        }

        public static RadialRect Inflate(RadialRect rect, int x, int y)
        {
            RadialRect r = rect;
            r.Inflate(x, y);
            return r;
        }

        public void Intersect(RadialRect rect)
        {
            RadialRect result = RadialRect.Intersect(rect, this);

            this.X = result.X;
            this.Y = result.Y;
            this.Width = result.Width;
            this.Height = result.Height;
        }

        public static RadialRect Intersect(RadialRect a, RadialRect b)
        {
            int x1 = Math.Max(a.X, b.X);
            int x2 = Math.Min(a.X + a.Width, b.X + b.Width);
            int y1 = Math.Max(a.Y, b.Y);
            int y2 = Math.Min(a.Y + a.Height, b.Y + b.Height);

            if (x2 >= x1 && y2 >= y1)
            {
                return new RadialRect(x1, y1, x2 - x1, y2 - y1);
            }
            return RadialRect.Empty;
        }

        [Pure]
        public bool IntersectsWith(RadialRect rect)
        {
            return (rect.X < this.X + this.Width) &&
            (this.X < (rect.X + rect.Width)) &&
            (rect.Y < this.Y + this.Height) &&
            (this.Y < rect.Y + rect.Height);
        }

        [Pure]
        public static RadialRect Union(RadialRect a, RadialRect b)
        {
            int x1 = Math.Min(a.X, b.X);
            int x2 = Math.Max(a.X + a.Width, b.X + b.Width);
            int y1 = Math.Min(a.Y, b.Y);
            int y2 = Math.Max(a.Y + a.Height, b.Y + b.Height);

            return new RadialRect(x1, y1, x2 - x1, y2 - y1);
        }

        public void Offset(Point pos)
        {
            Offset(pos.X, pos.Y);
        }

        public void Offset(int x, int y)
        {
            this.X += x;
            this.Y += y;
        }


        public static readonly RadialRect Empty = new RadialRect();


        public RadialRect(Point location, Size size)
        {
            _x = location.X;
            _y = location.Y;
            _width = size.Width;
            _height = size.Height;
        }


        public RadialRect(Point location, int width, int height)
        {
            _x = location.X;
            _y = location.Y;
            _width = width;
            _height = height;
        }


        public RadialRect(int x, int y, Size size)
        {
            _x = x;
            _y = y;
            _width = size.Width;
            _height = size.Height;
        }


        public RadialRect(
            int x, int y, 
            int width, int height)
        {
            _x = x;
            _y = y;
            _width = width;
            _height = height;
        }


        public bool Equals(Rectangle other)
        {
            return (Rectangle)this == other;
        }

        public bool Equals(RadialRect other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is RadialRect)) {
                return false;
            }
            return this.Equals((RadialRect)obj);
        }

        public override int GetHashCode()
        {
            int hashCode = 214636692;
            hashCode = hashCode * -1521134295 + _x.GetHashCode();
            hashCode = hashCode * -1521134295 + _y.GetHashCode();
            hashCode = hashCode * -1521134295 + _width.GetHashCode();
            hashCode = hashCode * -1521134295 + _height.GetHashCode();
            return hashCode;
        }

        public override string ToString()
        {
            return 
                "{X=" + X.ToString(CultureInfo.CurrentCulture) + 
                ",Y=" + Y.ToString(CultureInfo.CurrentCulture) +
                ",Width=" + Width.ToString(CultureInfo.CurrentCulture) +
                ",Height=" + Height.ToString(CultureInfo.CurrentCulture) + 
                ",X Zero=" + XZero.ToString(CultureInfo.CurrentCulture) +
                ",Y Zero=" + YZero.ToString(CultureInfo.CurrentCulture) + "}";
        }
    }
}
