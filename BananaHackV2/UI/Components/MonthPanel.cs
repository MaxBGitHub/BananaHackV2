using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace BananaHackV2.UI.Components
{
    internal class MonthPanel : Panel
    {
        private const int DAYSPERWEEK = 7;

        // The calendar should display the same amount of dates
        // for each month. We can ensure this by displaying 42 days
        // at any given time. 28 to 31 days for the actual month
        // and a couple of days to make sure we start a monday and
        // end at a sunday. It does not matter if the first monday
        // and the last sunday is within the current month bounds.
        private const int CALENDAR_DAYCOUNT = DAYSPERWEEK * 6;

        private const int COLUMNCOUNT   = DAYSPERWEEK;
        private const int ROWCOUNT      = CALENDAR_DAYCOUNT / DAYSPERWEEK;

        // Since normal human beings start their week on a monday
        // and not on a sunday like some animals we need to define
        // our days of week in new order as the enum is not a bit field...
        private readonly DayOfWeek[] _orderedDays = new DayOfWeek[DAYSPERWEEK] {
            DayOfWeek.Monday,
            DayOfWeek.Tuesday,
            DayOfWeek.Wednesday,
            DayOfWeek.Thursday,
            DayOfWeek.Friday,
            DayOfWeek.Saturday,
            DayOfWeek.Sunday,
        };

        private DateTime[]  _days       = new DateTime[CALENDAR_DAYCOUNT];
        private Rectangle[] _dayBounds  = new Rectangle[CALENDAR_DAYCOUNT];


        private StringFormat _format = new StringFormat() {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center,
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


        public struct DayFormat {
            public Color    ForeColor;
            public Color    BackColor;
            public Color    ActiveForeColor;
            public Color    ActiveBackColor;
            public Font     Font;
            public Font     ActiveFont;
        }


        DayFormat _defaultFormat;
        public DayFormat DefaultFormat
        {
            get {
                return _defaultFormat;
            }
            set {
                _defaultFormat = value;
            }
        }


        DayFormat _outOfMonthFormat;
        public DayFormat OutOfMonthFormat
        {
            get {
                return _outOfMonthFormat;
            }
            set {
                _outOfMonthFormat = value;
            }
        }



        private int _year;
        public int Year
        {
            get {
                return _year;
            }
            set {
                if (_year != value) {
                    _year = value;
                }
            }
        }


        private int _month;
        public int Month
        {
            get {
                return _month;
            }
            set {
                if (value != _month) {
                    _month = value;
                }
            }
        }


        private DateTime _firstDayOfMonth;
        public DateTime FirstDayOfMonth
        {
            get {
                return _firstDayOfMonth;
            }
        }



        private DateTime _lastDayOfMonth;
        public DateTime LastDayOfMonth
        {
            get {
                return _lastDayOfMonth;
            }
        }

        
        private void SetDayBounds()
        {
            _firstDayOfMonth = new DateTime(_year, _month, 1);
            _lastDayOfMonth = _firstDayOfMonth.AddMonths(1).AddDays(-1);

            DateTime first = _firstDayOfMonth;

            while (first.DayOfWeek != DayOfWeek.Monday) {
                first = first.AddDays(-1);
            }

            _days = new DateTime[CALENDAR_DAYCOUNT];
            for (int i = 0; i < CALENDAR_DAYCOUNT; i++) {
                _days[i] = first.AddDays(i);
            }
        }        


        protected override void OnPaint(PaintEventArgs e)
        {
            if (_dayBounds == null) {
                return;
            }

            if (_dayBounds.Length != CALENDAR_DAYCOUNT) {
                return;
            }

            using (var backBrush = new SolidBrush(_defaultFormat.BackColor))
            using (var foreBrush = new SolidBrush(_defaultFormat.ForeColor))
            using (var outBackBrush = new SolidBrush(_outOfMonthFormat.BackColor))
            using (var outForeBrush = new SolidBrush(_outOfMonthFormat.ForeColor))
            {
                for (int i = 0; i < _days.Length; i++)
                {
                    if (_days[i].Month == _month) {
                        e.Graphics.FillRectangle(backBrush, _dayBounds[i]);
                        e.Graphics.DrawString(_days[i].Day.ToString(), Font, foreBrush, _dayBounds[i], _format);
                    }
                    else {
                        e.Graphics.FillRectangle(outBackBrush, _dayBounds[i]);
                        e.Graphics.DrawString(_days[i].Day.ToString(), Font, outForeBrush, _dayBounds[i], _format);
                    }
                }
            }
        }


        private int _dw;
        private int _dh;

        private void InitTableLayout()
        {
            _dw = this.Width / COLUMNCOUNT;
            _dh = this.Height / ROWCOUNT;

            int index = -1;
            for (int y = 0; y < COLUMNCOUNT; y++) {
                for (int x = 0; x < ROWCOUNT; x++) {
                    int xc = x * _dw;
                    int yc = y * _dh;
                    _dayBounds[++index] = new Rectangle(
                        xc, yc,
                        _dw, _dh);
                }
            }
        }


        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            
            int mx = e.Location.X;
            int my = e.Location.Y;

            int x = mx / _dw;
            int y = my / _dh;

            Debug.WriteLine($"MOUSE: {x} - {y}");
        }


        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
        }


        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
        }


        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            InitTableLayout();
        }


        private void InitFormats()
        {
            _defaultFormat = new DayFormat() {
                ForeColor       = this.ForeColor,
                BackColor       = this.BackColor,
                Font            = this.Font,
                ActiveBackColor = this.BackColor,
                ActiveForeColor = SystemColors.ButtonHighlight,
                ActiveFont      = new Font(this.Font, FontStyle.Bold)
            };

            _outOfMonthFormat = new DayFormat() {
                ForeColor       = ApplyAlphaCompositing(60, this.ForeColor),
                BackColor       = this.BackColor,
                Font            = this.Font,
                ActiveBackColor = this.BackColor,
                ActiveForeColor = ApplyAlphaCompositing(60, SystemColors.ButtonHighlight),
                ActiveFont      = new Font(this.Font, FontStyle.Bold)
            };
        }


        public MonthPanel()
            : base()
        {
            this.DoubleBuffered = true;

            _year = DateTime.Today.Year;
            _month = DateTime.Today.Month;

            InitFormats();
            SetDayBounds();
            InitTableLayout();
        }

    }
}
