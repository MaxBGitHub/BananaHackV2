using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Data;

namespace BananaHackV2.UI.Components
{
    //
    // TODO:
    // =====
    //  - Move to month |-1 1| onClick() == outOfMonth.
    //  - Handle SelectedDay visual appreance when changing month or year
    //  - Paint week days
    //  - Paint month and year
    //  - Allow adding a contextMenu or control 
    //    onClick() == month and year label.
    //
    internal class MonthControl : Control
    {
        #region Member

        private static readonly Func<Array, int, bool> IsIndex = (a, i) => {
            return i > -1 && i < a.Length;
        };

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

        private const int MAX_YEAR = 9999;
        private const int MIN_YEAR =    1;

        private const int MAX_MONTH = 12;
        private const int MIN_MONTH =  1;

        private float _dayWidth;  // The width of a single day rect.        
        private float _dayHeight; // The height of single day rect.

        private ToolTip _dateTip;

        //private RectangleF _rcNonClient;
        //private RectangleF _rcNclMoveLeft;
        //private RectangleF _rcNclMoveRight;

        //private RectangleF _rcClient;


        private int _mouseOverIndex = -1;
        protected int MouseOverIndex
        {
            get {
                return _mouseOverIndex;
            }
            set {
                _mouseOverIndex = value;
            }
        }

        private int _selectedIndex = -1;
        protected int SelectedIndex
        {
            get {
                return _selectedIndex;
            }
            set {
                _selectedIndex = value;
                if (IsIndex(_days, _selectedIndex)) {
                    _selectedDate = _days[_selectedIndex];
                }                
            }
        }               


        private DateTime[]  _days = new DateTime[CALENDAR_DAYCOUNT];
        protected DateTime[] Days 
        { 
            get { 
                return _days; 
            } 
        }

        private RectangleF[] _dayBounds = new RectangleF[CALENDAR_DAYCOUNT];
        protected RectangleF[] DayBounds
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


        #region Events (custom and subscribable)

        protected event EventHandler onSelectedDayChanged;
        public event EventHandler SelectedDayChanged
        {
            add {
                onSelectedDayChanged += value;
            }
            remove {
                onSelectedDayChanged -= value;
            }
        }

        protected virtual void OnSelectedDayChanged()
        {
            onSelectedDayChanged?.Invoke(this, EventArgs.Empty);
        }


        protected event EventHandler onYearChanged;
        public event EventHandler YearChanged
        {
            add {
                onYearChanged += value;
            }
            remove {
                onYearChanged -= value;
            }
        }

        protected virtual void OnYearChanged()
        {
            onYearChanged?.Invoke(this, EventArgs.Empty);
        }


        protected event EventHandler onMonthChanged;
        public event EventHandler MonthChanged
        {
            add {
                onMonthChanged += value;
            }
            remove {
                onMonthChanged -= value;
            }
        }

        protected virtual void OnMonthChanged()
        {
            onMonthChanged?.Invoke(this, EventArgs.Empty);
        }


        private event EventHandler<PaintEventArgs> onPaintDays;
        public event EventHandler<PaintEventArgs> PaintDays
        {
            add {
                onPaintDays += value;
            }
            remove {
                onPaintDays -= value;
            }
        }


        private event EventHandler<PaintEventArgs> onPaintSelectedDay;
        public event EventHandler<PaintEventArgs> PaintSelectedDay
        {
            add {
                onPaintSelectedDay += value;
            }
            remove {
                onPaintSelectedDay -= value;
            }
        }


        private event EventHandler<PaintEventArgs> onPaintMouseOverDay;
        public event EventHandler<PaintEventArgs> PaintMouseOverDay
        {
            add {
                onPaintMouseOverDay += value;
            }
            remove {
                onPaintMouseOverDay -= value;
            }
        }


        private event EventHandler<PaintEventArgs> onPaintBorder;
        public event EventHandler<PaintEventArgs> PaintBorder
        {
            add {
                onPaintBorder += value;
            }
            remove {
                onPaintBorder -= value;
            }
        }


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
            if (this.Width == 0 && this.Height == 0) {
                return;
            }

            // The width for each day bounding rectangle.
            _dayWidth = (float)this.Width / (float)COLUMNCOUNT;
            // The height for each day bounding rectangle.
            _dayHeight = (float)this.Height / (float)ROWCOUNT;

            int index = -1;
            // Iterate over rows and columns
            // to calculate bounding rectangles
            // for each single day.
            for (int y = 0; y < ROWCOUNT; y++) {
                for (int x = 0; x < COLUMNCOUNT; x++) {
                    float xc = /* _rcClient.X + */ x * _dayWidth;
                    float yc = /* _rcClient.Y + */ y * _dayHeight;
                    _dayBounds[++index] = new RectangleF(
                        xc, yc, _dayWidth, _dayHeight);
                }
            }
        }

        #endregion


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
                Refresh();
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

        private Pen _highLightSelectedPen;
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
                Color highlightRectClr = ApplyAlphaCompositing(60, _highlightBackColor);
                _highLightSelectedPen?.Dispose();
                _highLightSelectedPen = new Pen(highlightRectClr) {
                    DashStyle = System.Drawing.Drawing2D.DashStyle.Dash
                };
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
        public bool AllowOutOfMonthSelection
        {
            get {
                return _allowOutOfMonth;
            }
            set {
                _allowOutOfMonth = value;
            }
        }


        private bool _freeze = false;
        /// <summary>
        /// Set to TRUE to disable the abillity 
        /// to change the year or month.
        /// </summary>
        public bool Freeze
        {
            get {
                return _freeze;
            }
            set {
                _freeze = value;
            }
        }


        private bool _showDateTip = false;
        public bool ShowDateTip
        {
            get {
                return _showDateTip;
            }
            set {
                _showDateTip = value;
                if (_showDateTip) {
                    _dateTip?.Dispose();
                    _dateTip = new ToolTip();
                }
                else {
                    _dateTip?.Dispose();
                    _dateTip = null;
                }
            }
        }


        private bool _renderOutOfMonth = true;
        public bool RenderOutOfMonth
        {
            get {
                return _renderOutOfMonth;
            }
            set {
                _renderOutOfMonth = value;
                Refresh();
            }
        }

        #endregion


        #region DateTime member

        private int GetSelectedIndex(DateTime? selected, int month, int year)
        {
            if (!(selected.HasValue)) {
                return -1;
            }

            int day = selected.Value.Day;
            if (day > DateTime.DaysInMonth(year, month)) {
                day = DateTime.DaysInMonth(year, month);
            }
            int index = _nFirst + day - 1;
            return index;
        }


        private void UpdateControl()
        {
            DateTime? sel = SelectedDate;
            SetDays();
            InitTableLayout();
            SelectedIndex = GetSelectedIndex(sel, _month, _year);
            Invalidate();
        }


        private int _year;
        public int Year
        {
            get {
                return _year;
            }
            set {
                if (_freeze || !(Enabled)) {
                    return;
                }

                if (value < MIN_YEAR || value > MAX_YEAR) {
                    throw new ArgumentOutOfRangeException(nameof(Year));
                }

                if (_year == value) {
                    return;
                }
                _year = value;
                UpdateControl();
                OnYearChanged();
            }
        }


        private int _month;
        public int Month
        {
            get {
                return _month;
            }
            set {
                
                if (_freeze || !(Enabled)) {
                    return;
                }

                if (value < MIN_MONTH || value > MAX_MONTH) {
                    throw new ArgumentOutOfRangeException(nameof(Month));
                }

                if (_month == value) {
                    return;
                }
                _month = value;
                UpdateControl();
                OnMonthChanged();
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
        private DateTime? _selectedDate;
        public DateTime? SelectedDate
        {
            get {
                return _selectedDate;
            }
            set {
                if (_freeze || !(Enabled)) {
                    return;
                }

                _selectedDate = value;
                if (_selectedDate != null) {                    
                    _year = _selectedDate.Value.Year;
                    _month = _selectedDate.Value.Month;
                    UpdateControl();
                    OnSelectedDayChanged();
                }
            }
        }

        #endregion


        #region Painting

        protected virtual void OnPaintDay(DateTime day, PaintEventArgs e)
        {            
            string sDay = day.Day.ToString();
            bool isMonth = day.Month == _month;
            if (isMonth) {                
                e.Graphics.FillRectangle(_backBrush, e.ClipRectangle);
                e.Graphics.DrawString(sDay,
                    Font,
                    _foreBrush,
                    e.ClipRectangle,
                    _format);
            }
            else if (!(isMonth) && _renderOutOfMonth) {
                e.Graphics.FillRectangle(_outOfMonthBackBrush, e.ClipRectangle);
                e.Graphics.DrawString(sDay,
                    _outOfMonthFont,
                    _outOfMonthForeBrush,
                    e.ClipRectangle,
                    _format);
            }
        }

        protected virtual void OnPaintDays(PaintEventArgs e) 
        {
            if (_dayBounds == null) {
                return;
            }

            if (_dayBounds.Length != CALENDAR_DAYCOUNT) {
                return;
            }

            for (int i = 0; i < _days.Length; i++) {
                Rectangle rc = Rectangle.Round(_dayBounds[i]);
                PaintEventArgs pargs = new PaintEventArgs(e.Graphics, rc);
                OnPaintDay(_days[i], pargs);
            }
        }


        const int PADDING_FOCUSRECT = -3;

        private static readonly Func<RectangleF, RectangleF> GetFocusRect = (rc) => {
            rc.Inflate(PADDING_FOCUSRECT, PADDING_FOCUSRECT);
            return rc;
        };


        private void PaintHighlightRect(PaintEventArgs e)
        {
            RectangleF rc = GetFocusRect(_dayBounds[SelectedIndex]);
            e.Graphics.DrawRectangle(
                _highLightSelectedPen,
                rc.X, rc.Y,
                rc.Width, rc.Height);
        }


        protected virtual void OnPaintMouseOverDay(PaintEventArgs e)
        {
            if (!(IsIndex(_dayBounds, MouseOverIndex))) {
                return;
            }

            if (MouseOverIndex == SelectedIndex) {
                PaintHighlightRect(e);
                return;
            }

            bool isMonth = _days[MouseOverIndex].Month == _month;
            if (!(isMonth) && (!(_allowOutOfMonth) || !(_renderOutOfMonth))) {
                return;
            }

            e.Graphics.FillRectangle(
                _highlightBackBrush, 
                _dayBounds[MouseOverIndex]);

            e.Graphics.DrawString(
                _days[MouseOverIndex].Day.ToString(),
                _highlightFont, 
                _highlightForeBrush, 
                _dayBounds[MouseOverIndex], 
                _format);
        }


        protected virtual void OnPaintSelectedDay(PaintEventArgs e)
        {
            if (!(IsIndex(_dayBounds, SelectedIndex))) {
                return;
            }

            e.Graphics.FillRectangle(
                _selectedDayBackBrush, 
                _dayBounds[SelectedIndex]);

            e.Graphics.DrawString(
                _days[SelectedIndex].Day.ToString(),
                Font, 
                _selectedDayForeBrush, 
                _dayBounds[SelectedIndex], 
                _format);
        }


        private void PaintHorizontalBorder(PaintEventArgs e)
        {
            for (int y = 0; y < CALENDAR_DAYCOUNT - (ROWCOUNT + 1); y += DAYSPERWEEK) {
                // Index of last day in the current row.
                int yc = y + DAYSPERWEEK - 1;
                // Start at first day in row.
                PointF ptStart = new PointF(
                    _dayBounds[y].Left, 
                    _dayBounds[y].Bottom);
                // End at last day in current row.
                PointF ptEnd = new PointF(
                    _dayBounds[yc].Right, 
                    _dayBounds[yc].Bottom);
                    
                e.Graphics.DrawLine(_borderPen, ptStart, ptEnd);
            }
        }


        private void PaintVerticalBorder(PaintEventArgs e)
        {
            for (int x = 0; x < COLUMNCOUNT - 1; x++) {
                // Index of last day in the current column.
                int xc = x + DAYSPERWEEK * (ROWCOUNT - 1);
                // Start at first day in current column.
                PointF ptStart = new PointF(
                    _dayBounds[x].Right, 
                    _dayBounds[x].Top);
                // End at last day in current column.
                PointF ptEnd = new PointF(
                    _dayBounds[xc].Right, 
                    _dayBounds[xc].Bottom);

                e.Graphics.DrawLine(_borderPen, ptStart, ptEnd);
            }
        }


        protected virtual void OnPaintBorder(PaintEventArgs e)
        {
            // Paint horizontal border lines.
            // The first and the last border line is omitted.
            if ((DayBorder.Horizontal & _border) == DayBorder.Horizontal) {
                PaintHorizontalBorder(e);
            }

            // Paint vertical border lines.
            // The first and the last border line is omitted.
            if ((DayBorder.Vertical & _border) == DayBorder.Vertical) {
                PaintVerticalBorder(e);   
            }
        }


        // 
        // The order of which components are painted first is relevant.
        // First step is to just paint all days in general.
        // Second step is to paint the day where the mouse is currently over.
        // Third step is to paint the selected / clicked day.
        // Forth and last step is to paint the border.
        //
        // If we paint the border first it would be painted over by the other 
        // paint handlers and would get lost.
        //
        protected override void OnPaint(PaintEventArgs e)
        {
            OnPaintDays(e);
            onPaintDays?.Invoke(this, e);

            OnPaintSelectedDay(e);
            onPaintSelectedDay?.Invoke(this, e);

            OnPaintMouseOverDay(e);
            onPaintMouseOverDay?.Invoke(this, e);

            OnPaintBorder(e);
            onPaintBorder?.Invoke(this, e);
        }

        #endregion


        #region Mouse handling and reactive painting

        private int GetIndexFromLocation(PointF pt)
        {
            // Get x and y position in table / grid.
            int x = (int)(pt.X / _dayWidth);
            int y = (int)(pt.Y / _dayHeight);

            int index = y * DAYSPERWEEK + x;
            
            return index;
        }


        private void TryShowDateTip(int index)
        {
            bool isMonth = _days[index].Month == _month;
            if (isMonth || (!(isMonth) && (_renderOutOfMonth && _allowOutOfMonth)))
            {
                _dateTip?.Show(
                   _days[index].ToShortDateString(),
                this,
                   (int)_dayBounds[index].Left,
                   (int)_dayBounds[index].Bottom);
            }
            else
            {
                _dateTip?.Hide(this);
            }
        }


        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);            

            int index = GetIndexFromLocation(e.Location);
            if (index == -1) {
                return;
            }

            if (index >= _days.Length) {
                index = _days.Length - 1;
            }

            if ((MouseOverIndex == -1 && index != -1) || (MouseOverIndex != index)) {
                MouseOverIndex = index;
                TryShowDateTip(index);
            }
            Invalidate();
        }


        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            PointF pt = this.PointToClient(Cursor.Position);
            int index = GetIndexFromLocation(pt);

            if (index >= CALENDAR_DAYCOUNT) {
                MouseOverIndex = -1;
            }
            else {
                MouseOverIndex = index;
                Invalidate();
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

            int index = GetIndexFromLocation(e.Location);
            if (!(IsIndex(_days, index))) {
                SelectedIndex = -1;
                return;
            }

            if (!(AllowOutOfMonthSelection) && _days[index].Month != _month) {
                return;
            }

            if (SelectedIndex == -1) {
                SelectedIndex = index;
                _selectedDate = _days[SelectedIndex];                
            }
            else {
                SelectedIndex = index;
                _selectedDate = _days[SelectedIndex];
                
            }
            Invalidate();
        }

        #endregion


        #region HitTest

        public DayHitTest HitTest(Point location)
        {
            int index = GetIndexFromLocation(location);
            if (!(index >= 0 && index < _days.Length)) {
                return DayHitTest.Empty;
            }

            int row = (int)Math.Floor((float)index / (float)DAYSPERWEEK);
            int col = index - (row * DAYSPERWEEK);

            return new DayHitTest(
                index, row, col, 
                _dayBounds[index], 
                _days[index], 
                _days[index].Month == _month);
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


        private void InitializeFormats()
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


        private void Initialize()
        {
            _selectedDate = DateTime.Today;
            _year = _selectedDate.Value.Year;
            _month = _selectedDate.Value.Month;
        }


        private void SetDate()
        {
            SelectedIndex = GetSelectedIndex(_selectedDate, _month, _year);
        }


        public MonthControl()
            : base()
        {
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);

            this.DoubleBuffered = true;

            Initialize();
            InitializeFormats();
            SetDays();
            InitTableLayout();

            SetDate();
        }


        #region HitTest

        public class DayHitTest
        {
            public int Index { get; }
            public int RowIndex { get; }
            public int ColumnIndex { get; }
            public RectangleF Bounds { get; }
            public DateTime Day { get; }
            public bool IsDisplayedMonth { get; }

            public static DayHitTest Empty
            {
                get {
                    return new DayHitTest(
                        -1, -1, -1, 
                        RectangleF.Empty, 
                        DateTime.MinValue, 
                        false);
                }
            }

            internal DayHitTest(
                int index, 
                int row, 
                int col, 
                RectangleF bounds, 
                DateTime day, 
                bool isMonth)
            {
                Index = index;
                RowIndex = row;
                ColumnIndex = col;
                Bounds = bounds;
                Day = day;
                IsDisplayedMonth = isMonth;
            }
        }

        #endregion
    }
}
