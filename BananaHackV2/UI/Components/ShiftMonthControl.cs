using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;


namespace BananaHackV2.UI.Components
{
    //
    // TODO:
    // =====
    //  - Handle SelectedDay visual appreance when changing month or year
    //  = Allow adding and removing ShiftType enums to days
    //  - Display shifts with user defined colors
    //  - Display shifts on mouse hover
    //  - Edit shifts during runtime via context menu
    //  - Add events     
    //      - OnSelectedDayChanged
    //      - OnMonthChanged
    //      - OnYearChanged
    //      - OnShiftChanged
    //
    internal class ShiftMonthControl : Control
    {
        #region Member

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

        protected int MouseOverIndex = -1;
        protected int SelectedIndex = -1;

        private int _dayWidth;
        private int _dayHeight;

        //// Since normal human beings start their week on a monday
        //// and not on a sunday like some animals we need to define
        //// our days of week in new order as the enum is not a bit field...
        //protected DayOfWeek[] DaysOfWeek = new DayOfWeek[DAYSPERWEEK] {
        //    DayOfWeek.Monday,
        //    DayOfWeek.Tuesday,
        //    DayOfWeek.Wednesday,
        //    DayOfWeek.Thursday,
        //    DayOfWeek.Friday,
        //    DayOfWeek.Saturday,
        //    DayOfWeek.Sunday,
        //};

        private DateTime[]  _days = new DateTime[CALENDAR_DAYCOUNT];
        protected DateTime[] Days 
        { 
            get { 
                return _days; 
            } 
        }

        private Rectangle[] _dayBounds = new Rectangle[CALENDAR_DAYCOUNT];
        protected Rectangle[] DayBounds
        {
            get {
                return _dayBounds;
            }
        }        


        private StringFormat _format = new StringFormat() {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center,
        };

        #endregion


        #region Days and bounds calculation

        private void SetDays()
        {
            // First day of the specified month.
            _firstDayOfMonth = new DateTime(_year, _month, 1);
            // Last day of the specified month.
            _lastDayOfMonth = _firstDayOfMonth.AddMonths(1).AddDays(-1);

            // Copy first day for further calculations.
            DateTime first = _firstDayOfMonth;

            _nFirst = 0;
            // If first of month is not a monday
            // decrement first until we get a monday.
            // The control will always display 42 days
            // to have a unified view for each month.
            // In short, we display by week and not 
            // thus need to normalize our start and 
            // end point of the calendar. To normalize 
            // we have to find a monday before the first 
            // day of the current month if the first day 
            // isn't already a monday.
            while (first.DayOfWeek != DayOfWeek.Monday) {
                first = first.AddDays(-1);
                // Increment index of first day of month.
                // _nFirst is always set to the index 
                // of the actual first day of the month and
                // not the index of the first displayed day.
                // We can simply increment the index each time
                // we decrement the day when looking for 
                // the first monday of the first week.
                _nFirst++;
            }

            // _nLast is always set to the index of 
            // the actual last day of the month and not the 
            // index of the last displayed day.
            // We can get that index by simply adding the
            // actual day of the last day of the month and 
            // decrementing by 1 as we start at index 0.
            _nLast = _nFirst + (_lastDayOfMonth.Day) - 1;

            // Set all available dates by adding i to the
            // first displayed day.
            _days = new DateTime[CALENDAR_DAYCOUNT];
            for (int i = 0; i < CALENDAR_DAYCOUNT; i++) {
                _days[i] = first.AddDays(i);
            }
        }


        private void InitTableLayout()
        {
            // The width for each day bounding rectangle.
            _dayWidth = this.Width / COLUMNCOUNT;
            // The height for each day bounding rectangle.
            _dayHeight = this.Height / ROWCOUNT;

            int index = -1;
            // Iterate over rows and columns
            // to calculate bounding rectangles
            // for each single day.
            for (int y = 0; y < ROWCOUNT; y++) {
                for (int x = 0; x < COLUMNCOUNT; x++) {
                    int xc = x * _dayWidth;
                    int yc = y * _dayHeight;
                    _dayBounds[++index] = new Rectangle(
                        xc, yc, _dayWidth, _dayHeight);
                }
            }
        }

        #endregion


        private Dictionary<DateTime, ShiftType> _shifts = new Dictionary<DateTime, ShiftType>();
        public void AddShift(int day, ShiftType shiftType)
        {
            DateTime target = new DateTime(_year, _month, day);
            //int offset = 
        }


        #region Border

        [Flags]
        public enum DayBorder
        {
            None        = 0,
            Horizontal  = 1 << 1,
            Vertical    = 1 << 2,
            HorizontalVertical = Horizontal | Vertical,            
        }


        private DayBorder _border = DayBorder.None;
        public DayBorder BorderStyle 
        {
            get {
                return _border;
            }
            set {
                _border = value;
            }
        }


        private int _borderThickness = 1;
        public int BorderThickness
        {
            get {
                return _borderThickness;
            }
            set {
                _borderThickness = value;
                _borderPen?.Dispose();
                _borderPen = new Pen(_borderColor, _borderThickness);
            }
        }


        private Pen _borderPen;
        private Color _borderColor;
        public Color BorderColor
        {
            get {
                return _borderColor;
            }
            set {
                _borderColor = value;
                _borderPen?.Dispose();
                _borderPen = new Pen(_borderColor, _borderThickness);
            }
        }

        #endregion


        #region Appearance

        private Brush _foreBrush;
        public override Color ForeColor
        {
            get {
                return base.ForeColor;
            }
            set {
                base.ForeColor = value;
                _foreBrush?.Dispose();
                _foreBrush = new SolidBrush(base.ForeColor);
            }
        }

        private Brush _backBrush;
        public override Color BackColor
        {
            get {
                return base.BackColor;
            }
            set {
                base.BackColor = value;
                _backBrush?.Dispose();
                _backBrush = new SolidBrush(base.BackColor);
            }
        }


        private Font _highlightFont;
        public Font HighlightFont
        {
            get {
                return _highlightFont;
            }
            set {
                _highlightFont = value;
            }
        }

        private Brush _highlightForeBrush;
        private Color _highlightForeColor;
        public Color HighlightForeColor
        {
            get {
                return _highlightForeColor;
            }
            set {
                _highlightForeColor = value;
                _highlightForeBrush?.Dispose();
                _highlightForeBrush = new SolidBrush(_highlightForeColor);
            }
        }

        private Brush _highlightBackBrush;
        private Color _highlightBackColor;
        public Color HighlightBackColor
        {
            get {
                return _highlightBackColor;
            }
            set {
                _highlightBackColor = value;
                _highlightBackBrush?.Dispose();
                _highlightBackBrush = new SolidBrush(_highlightBackColor);
            }
        }


        private Font _outOfMonthFont;
        public Font OutOfMonthFont
        {
            get {
                return _outOfMonthFont;
            }
            set {
                _outOfMonthFont = value;
            }
        }

        private Brush _outOfMonthForeBrush;
        private Color _outOfMonthForeColor;
        public Color OutOfMonthForeColor
        {
            get {
                return _outOfMonthForeColor;
            }
            set {
                _outOfMonthForeColor = value;
                _outOfMonthForeBrush?.Dispose();
                _outOfMonthForeBrush = new SolidBrush(_outOfMonthForeColor);
            }
        }

        private Brush _outOfMonthBackBrush;
        private Color _outOfMonthBackColor;
        public Color OutOfMonthBackColor
        {
            get {
                return _outOfMonthBackColor;
            }
            set {
                _outOfMonthBackColor = value;
                _outOfMonthBackBrush?.Dispose();
                _outOfMonthBackBrush = new SolidBrush(_outOfMonthBackColor);
            }
        }

        private Brush _selectedDayForeBrush;
        private Color _selectedDayForeColor;
        public Color SelectedDayForeColor
        {
            get {
                return _selectedDayForeColor;
            }
            set {
                _selectedDayForeColor = value;
                _selectedDayForeBrush?.Dispose();
                _selectedDayForeBrush = new SolidBrush(_selectedDayForeColor);
            }
        }

        private Brush _selectedDayBackBrush;
        private Color _selectedDayBackColor;
        public Color SelectedDayBackColor
        {
            get {
                return _selectedDayBackColor;
            }
            set
            {
                _selectedDayBackColor = value;
                _selectedDayBackBrush?.Dispose();
                _selectedDayBackBrush = new SolidBrush(_selectedDayBackColor);
            }
        }

        #endregion


        #region Behaviour

        private bool _allowOutOfMonth = false;
        public bool AllowOutOfMonth
        {
            get {
                return _allowOutOfMonth;
            }
            set {
                _allowOutOfMonth = value;
            }
        }

        #endregion


        #region DateTime member

        private int _year;
        public int Year
        {
            get {
                return _year;
            }
            set {
                if (_year != value) {
                    _year = value;
                    SetDays();
                    InitTableLayout();
                    Invalidate();
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
                
                if (value < 1 || value > 12) {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                if (value != _month) {
                    _month = value;
                    SetDays();
                    InitTableLayout();
                    Invalidate();
                }
            }
        }


        private int _nFirst;
        private DateTime _firstDayOfMonth;
        public DateTime FirstDayOfMonth
        {
            get {
                return _firstDayOfMonth;
            }
        }


        private int _nLast;
        private DateTime _lastDayOfMonth;
        public DateTime LastDayOfMonth
        {
            get {
                return _lastDayOfMonth;
            }
        }


        private int _nSelected;
        private DateTime _selectedDate;
        public DateTime SelectedDate
        {
            get {
                return _selectedDate;
            }
        }

        #endregion


        #region Painting

        protected virtual void OnPaintDays(PaintEventArgs e) 
        {
            if (_dayBounds == null) {
                return;
            }

            if (_dayBounds.Length != CALENDAR_DAYCOUNT) {
                return;
            }


            for (int i = 0; i < _days.Length; i++) {
                string day = _days[i].Day.ToString();
                if (_days[i].Month == _month) {
                    e.Graphics.FillRectangle(_backBrush, _dayBounds[i]);
                    e.Graphics.DrawString(day, Font, _foreBrush, _dayBounds[i], _format);
                }
                else {
                    e.Graphics.FillRectangle(_outOfMonthBackBrush, _dayBounds[i]);
                    e.Graphics.DrawString(day, _outOfMonthFont, _outOfMonthForeBrush, _dayBounds[i], _format);
                }
            }
        }


        protected virtual void OnPaintMouseOverDay(PaintEventArgs e)
        {
            if (MouseOverIndex != -1) {
                if (!(_allowOutOfMonth) && _days[MouseOverIndex].Month != _month) {
                    return;
                }
                e.Graphics.FillRectangle(_highlightBackBrush, _dayBounds[MouseOverIndex]);
                e.Graphics.DrawString(_days[MouseOverIndex].Day.ToString(),
                    _highlightFont, _highlightForeBrush, _dayBounds[MouseOverIndex], _format);
            }
        }


        protected virtual void OnPaintSelectedDay(PaintEventArgs e)
        {
            if (SelectedIndex != -1) {
                e.Graphics.FillRectangle(_selectedDayBackBrush, _dayBounds[SelectedIndex]);
                e.Graphics.DrawString(_days[SelectedIndex].Day.ToString(),
                    Font, _selectedDayForeBrush, _dayBounds[SelectedIndex], _format);
            }
        }


        protected virtual void OnPaintBorder(PaintEventArgs e)
        {
            if ((DayBorder.Horizontal & _border) == DayBorder.Horizontal) {
                for (int y = 0; y < CALENDAR_DAYCOUNT - (ROWCOUNT + 1); y += DAYSPERWEEK) {
                    int yc = y + DAYSPERWEEK - 1;
                    Point ptStart = new Point(_dayBounds[y].Left, _dayBounds[y].Bottom);
                    Point ptEnd = new Point(_dayBounds[yc].Right, _dayBounds[yc].Bottom);
                    e.Graphics.DrawLine(_borderPen, ptStart, ptEnd);
                }
            }

            if ((DayBorder.Vertical & _border) == DayBorder.Vertical) {
                for (int x = 0; x < COLUMNCOUNT - 1; x++) {
                    int xc = x + DAYSPERWEEK * (ROWCOUNT - 1);
                    Point ptStart = new Point(_dayBounds[x].Right, _dayBounds[x].Top);
                    Point ptEnd = new Point(_dayBounds[xc].Right, _dayBounds[xc].Bottom);
                    e.Graphics.DrawLine(_borderPen, ptStart, ptEnd);
                }
            }
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            OnPaintDays(e);
            OnPaintMouseOverDay(e);
            OnPaintBorder(e);
            OnPaintSelectedDay(e);
        }

        #endregion


        #region Mouse handling and reactive painting

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            int x = e.Location.X / _dayWidth;
            int y = e.Location.Y / _dayHeight;
            int index = y * DAYSPERWEEK + x;

            if (MouseOverIndex == -1) {
                Invalidate(_dayBounds[index]);
                MouseOverIndex = index;
            }

            if (MouseOverIndex != index) {
                Invalidate(_dayBounds[MouseOverIndex]);                
                Invalidate(_dayBounds[index]);
                MouseOverIndex = index;
            }
        }


        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            Point pt = Cursor.Position;
            pt = this.PointToClient(pt);

            int x = pt.X / _dayWidth;
            int y = pt.Y / _dayHeight;
            int index = y * DAYSPERWEEK + x;
            if (index > CALENDAR_DAYCOUNT) {
                MouseOverIndex = -1;
            }
            else {
                MouseOverIndex = index;
                Invalidate(_dayBounds[index]);
            }
        }


        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            MouseOverIndex = -1;
            Invalidate();
        }


        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            int x = e.X / _dayWidth;
            int y = e.Y / _dayHeight;
            int index = y * DAYSPERWEEK + x;
            if (index > CALENDAR_DAYCOUNT) {
                SelectedIndex = -1;
                return;
            }

            if (!(AllowOutOfMonth) && _days[index].Month != _month) {
                return;
            }

            if (SelectedIndex == -1) {
                SelectedIndex = index;
                _selectedDate = _days[SelectedIndex];
                Invalidate(_dayBounds[SelectedIndex]);
            }
            else {
                Invalidate(_dayBounds[SelectedIndex]);
                SelectedIndex = index;
                _selectedDate = _days[SelectedIndex];
                Invalidate(_dayBounds[SelectedIndex]);
            }
        }

        #endregion


        /// <summary>
        /// Update day bounding rects each time the 
        /// control is being resized.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            InitTableLayout();           
        }


        private void InitFormats()
        {
            _foreBrush = new SolidBrush(this.ForeColor);
            _backBrush = new SolidBrush(this.BackColor);

            SelectedDayBackColor = SystemColors.Highlight;
            SelectedDayForeColor = SystemColors.HighlightText;

            _highlightFont = new Font(this.Font, FontStyle.Bold);
            HighlightForeColor = SystemColors.ActiveCaption;
            HighlightBackColor = SystemColors.ButtonHighlight;

            _outOfMonthFont = this.Font;
            OutOfMonthForeColor = SystemColors.GrayText;
            OutOfMonthBackColor = this.BackColor;

            BorderStyle = DayBorder.HorizontalVertical;
            BorderColor = this.ForeColor;
        }


        public ShiftMonthControl()
            : base()
        {
            this.DoubleBuffered = true;

            _selectedDate   = DateTime.Today;
            _year           = _selectedDate.Year;
            _month          = _selectedDate.Month;            

            InitFormats();
            SetDays();
            InitTableLayout();
        }

    }
}
