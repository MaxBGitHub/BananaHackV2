using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace BananaHackV2.UI.Components
{
    internal class UploadBox : Panel
    {
        private const int MIN_PADDING = 12;

        private Padding GetPadding(Padding padding)
        {
            if (padding.Left < MIN_PADDING) {
                padding.Left = MIN_PADDING;
            }

            if (padding.Right < MIN_PADDING) {
                padding.Right = MIN_PADDING;
            }

            if (padding.Bottom < MIN_PADDING) {
                padding.Bottom = MIN_PADDING;
            }

            if (padding.Top < MIN_PADDING) {
                padding.Top = MIN_PADDING;
            }

            return padding;
        }


        public new Padding Padding
        {
            get {
                return base.Padding;
            }
            set {                
                base.Padding = GetPadding(value);
            }
        }


        public new BorderStyle BorderStyle
        {
            get {
                return BorderStyle.None;
            }
            set {
                // Nope.
            }
        }       


        private GraphicsPath GetRoundedRect(Rectangle bounds, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            if (radius == 0) {
                path.AddRectangle(bounds);
                return path;
            }

            int diameter = radius * 2;
            Size sz = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(bounds.Location, sz);

            path.AddArc(arc, 180, 90);

            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);

            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }


        private void SetHighQualityRendering(PaintEventArgs e)
        {
            e.Graphics.InterpolationMode    = InterpolationMode.HighQualityBicubic;
            e.Graphics.CompositingQuality   = CompositingQuality.HighQuality;
            e.Graphics.PixelOffsetMode      = PixelOffsetMode.HighQuality;
            e.Graphics.SmoothingMode        = SmoothingMode.HighQuality;
        }


        private int GetRadius(Rectangle bounds)
        {
            const int DEFAULTRADIUS = 32;
            int minBounds = Math.Min(bounds.Width, bounds.Height);
            int radius = Math.Min(minBounds, DEFAULTRADIUS);

            return radius;
        }


        private void PaintBorder(PaintEventArgs e)
        {
            var bounds = e.ClipRectangle;
            bounds.Inflate(-2, -2);

            int radius = GetRadius(bounds);

            SetHighQualityRendering(e);
            using (var dashPen = new Pen(Color.Black, 2f)) {
                dashPen.DashStyle = DashStyle.Dash;
                using (var path = GetRoundedRect(bounds, radius)) {
                    e.Graphics.DrawPath(dashPen, path);
                }
            }
        }


        private void PaintBackgroundInternal(PaintEventArgs e)
        {
            var bounds = e.ClipRectangle;
            bounds.Inflate(-2, -2);

            int radius = GetRadius(bounds);
            var backPath = GetRoundedRect(bounds, radius);

            SetHighQualityRendering(e);

            using (var parentBrush = new SolidBrush(Color.Transparent)) {
                e.Graphics.FillRectangle(parentBrush, e.ClipRectangle);
            }

            using (var brush = new SolidBrush(BackColor)) {
                e.Graphics.FillPath(brush, backPath);
            }
        }


        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (DesignMode) {
                base.OnPaintBackground(e);
                PaintBackgroundInternal(e);
                PaintBorder(e);
                return;
            }

            //base.OnPaintBackground(e);
            if (InvokeRequired) {
                this.Invoke(new MethodInvoker(() => {
                    PaintBackgroundInternal(e);
                    PaintBorder(e);
                }));
            }
            else {
                PaintBackgroundInternal(e);
                PaintBorder(e);
            }            
        }


        public UploadBox() 
        {
            base.BorderStyle = BorderStyle.None;
            base.Padding = new Padding(MIN_PADDING);
        }
    }
}
