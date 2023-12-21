using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BananaHackV2.UI.Components
{
    public partial class FlowImageContainer : UserControl
    {
        private Rectangle _rcClose = new Rectangle(0, 0, 24, 24);
        private Brush _normalBackBrush = Brushes.Red;
        private Brush _activeBackBrush = Brushes.IndianRed;
        private ToolTip _closeButtonTip;
        private bool _mouseOver = false;        


        public Image Image
        {
            get {
                return pb_ImageDisplay.Image;
            }
            set {
                pb_ImageDisplay.Image = value;
            }
        }


        private void OnMouseLeft(object sender, EventArgs e)
        {
            _mouseOver = false;
            pb_ImageDisplay.Invalidate();
        }


        private void OnMouseOverClose(object sender, MouseEventArgs e)
        {
            if (_mouseOver == false && _rcClose.Contains(e.Location)) {
                _mouseOver = true;
                _closeButtonTip.Show("Remove", this, e.Location, 750);
                pb_ImageDisplay.Invalidate();
            }
            else if (_mouseOver && !(_rcClose.Contains(e.Location))) {
                _mouseOver = false;
                _closeButtonTip.Hide(this);
                pb_ImageDisplay.Invalidate();
            }
        }


        private event EventHandler onCloseClicked;
        public event EventHandler CloseClicked
        {
            add {
                onCloseClicked += value;
            }
            remove {
                onCloseClicked -= value;
            }
        }


        private void OnCloseClick(object sender, MouseEventArgs e)
        {
            if (_rcClose.Contains(e.Location) && e.Button == MouseButtons.Left) {
                _closeButtonTip?.Dispose();
                onCloseClicked?.Invoke(this, EventArgs.Empty);
            }
        }


        private void PaintCloseButton(PaintEventArgs e)
        {
            if (_mouseOver) {
                e.Graphics.FillRectangle(_activeBackBrush, _rcClose);
                this.Cursor = Cursors.Hand;
            }            
            else {
                e.Graphics.FillRectangle(_normalBackBrush, _rcClose);
                this.Cursor = Cursors.Default;
            }

            using (var drawPen = new Pen(Color.White, 3f)) {                
                Point ptMidLeft = new Point(4, _rcClose.Height / 2);
                Point ptMidRight = new Point(_rcClose.Width - 4, _rcClose.Height / 2);
                e.Graphics.DrawLine(drawPen, ptMidLeft, ptMidRight);
            }
        }


        private void PictureBoxPaint(object sender, PaintEventArgs e)
        {
            PaintCloseButton(e);
        }


        public FlowImageContainer()
        {
            InitializeComponent();

            pb_ImageDisplay.Paint       += PictureBoxPaint;
            pb_ImageDisplay.MouseClick  += OnCloseClick;
            pb_ImageDisplay.MouseLeave  += OnMouseLeft;
            pb_ImageDisplay.MouseMove   += OnMouseOverClose;

            _closeButtonTip = new ToolTip();
        }
    }
}
