using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace BananaHackV2.UI.Components
{
    internal class CalendarControl : Control
    {
        const int CONTROL_HEIGHT = 36;

        Panel _pnMonthYearContainer = new Panel() {
            Name        = nameof(_pnMonthYearContainer),
            Dock        = DockStyle.Top,
            Height      = CONTROL_HEIGHT,
            AutoScroll  = false
        };

        Label _lblMonthYearDisplay = new Label {
            Name        = nameof(_lblMonthYearDisplay),
            AutoSize    = false,
            TextAlign = ContentAlignment.MiddleCenter
        };

        Label _btnMoveLeft = new Label() {
            Name = nameof(_btnMoveLeft),
            Text = "<",
            Size = new Size(CONTROL_HEIGHT, CONTROL_HEIGHT),
            TextAlign = ContentAlignment.MiddleCenter
        };

        Label _btnMoveRight = new Label() {
            Name = nameof(_btnMoveRight),
            Text = ">",
            Size = new Size(CONTROL_HEIGHT, CONTROL_HEIGHT),
            TextAlign = ContentAlignment.MiddleCenter
        };

        MonthControl _monthControl = new MonthControl() {
            Dock = DockStyle.Fill
        };


        private static readonly Func<byte, Color, Color> ApplyAlphaCompositing = (a, c) =>
        {
            double linAlpha = a / 255.0;
            int linR = 255 - c.R;
            int linG = 255 - c.G;
            int linB = 255 - c.B;
            var result = Color.FromArgb(
                (int)(linR * linAlpha + c.R),
                (int)(linG * linAlpha + c.G),
                (int)(linB * linAlpha + c.B));
            return result;
        };


        private void LabelMouseEnter(object sender, EventArgs e)
        {
            Label me = sender as Label;
            if (me == null) {
                return;
            }
            me.BackColor = SystemColors.ButtonHighlight;
        }


        private void LabelMouseLeave(object sender, EventArgs e)
        {
            Label me = sender as Label;
            if (me == null) {
                return;
            }

            me.BackColor = SystemColors.ButtonFace;
        }


        private void SetLabelLocations()
        {
            int wl = _lblMonthYearDisplay.Width;
            int w = this.Width;

            if (wl > w) {
                _lblMonthYearDisplay.Visible = false;
            }
            else {
                _lblMonthYearDisplay.Visible = true;
            }

            Size sz = TextRenderer.MeasureText(_lblMonthYearDisplay.Text, Font);
            _lblMonthYearDisplay.Size = new Size(sz.Width, _pnMonthYearContainer.Height);

            int mid = w / 2;
            Point pt = new Point(mid - wl / 2, 0);
            _lblMonthYearDisplay.Location = pt;

            pt.Offset(-(_btnMoveLeft.Width), 0);
            _btnMoveLeft.Size = new Size(CONTROL_HEIGHT, _pnMonthYearContainer.Height);
            _btnMoveLeft.Location = pt;

            pt = _lblMonthYearDisplay.Location;
            pt.Offset(_lblMonthYearDisplay.Width, 0);
            _btnMoveRight.Location = pt;
            _btnMoveRight.Size = new Size(CONTROL_HEIGHT, _pnMonthYearContainer.Height);
        }


        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            SetLabelLocations();

            for (int i = 0; i < Width; i+= 30)
            {
                for (int j = 0; j < Height; j+= 20)
                {
                    _monthControl.HitTest(new Point(i, j));
                }
            }
            
        }


        public CalendarControl()
            : base()
        {
            _lblMonthYearDisplay.Text = _monthControl.SelectedDate.Value.ToString("MMMM yyyy");

            _btnMoveLeft.Font = new Font(Font, FontStyle.Bold);
            _btnMoveRight.Font = new Font(Font, FontStyle.Bold);

            _btnMoveLeft.MouseEnter     += LabelMouseEnter;
            _btnMoveRight.MouseEnter    += LabelMouseEnter;

            _btnMoveLeft.MouseLeave     += LabelMouseLeave;
            _btnMoveRight.MouseLeave    += LabelMouseLeave;

            _pnMonthYearContainer.Controls.Add(_btnMoveLeft);
            _pnMonthYearContainer.Controls.Add(_lblMonthYearDisplay);
            _pnMonthYearContainer.Controls.Add(_btnMoveRight);
            
            Controls.Add(_monthControl);
            Controls.Add(_pnMonthYearContainer);

            SetLabelLocations();
        }
    }
}
