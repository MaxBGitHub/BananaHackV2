using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using Tesseract;
using System.Globalization;
using System.Drawing.Imaging;
using System.Security.Cryptography;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace BananaHackV2.OCR
{
    internal class ShiftProcessor : IDisposable
    {
        #region Member and consts

        private readonly TesseractEngine _engine;
        private readonly Pix    _pix;
        private readonly Bitmap _bitmap;

        private Dictionary<byte, ShiftType> _shiftTypeColors;

        private byte _lumaLate     = 0;
        private byte _lumaMorning  = 0;
        private byte _lumaOnCall   = 0;
        private byte _lumaSchool   = 0;

        private byte[] _colorRefs = null;

        
        private const int DEFAULT_TOLERANCE = 6;
        private const int DIRECTION_UP      = 1;
        private const int DIRECTION_DOWN    = -(DIRECTION_UP);


        private const double YUVWEIGTHED_R = 0.299;
        private const double YUVWEIGTHED_G = 0.587;
        private const double YUVWEIGTHED_B = 0.114;


        private const int MAXRGB = 256;
        private const int BYTESIZE = 8;

        private const int MIN_WIDTHTHRESHOLD = 12;


        private const string DEFAULT_LANGUAGE = "deu";
        private const string PATH_TESSDATA = "./tessdata";


        private const string RGX_NUMPATTERN = @"\((\d{1})+\)";

        private readonly Regex _numRegex = new Regex(
            RGX_NUMPATTERN, 
            RegexOptions.Compiled);

        #endregion


        private event EventHandler<OcrRegionArgs> onMonthRegionFound;
        public event EventHandler<OcrRegionArgs> MonthRegionFound {
            add {
                onMonthRegionFound += value;
            }
            remove {
                onMonthRegionFound -= value;
            }
        }


        private event EventHandler<OcrRegionArgs> onPersonRegionFound;
        public event EventHandler<OcrRegionArgs> PersonRegionFound {
            add {
                onPersonRegionFound += value;
            }
            remove {
                onPersonRegionFound -= value;
            }
        }


        private event EventHandler<OcrRegionArgs> onShiftRegionFound;
        public event EventHandler<OcrRegionArgs> ShiftRegionFound {
            add {
                onShiftRegionFound += value;
            }
            remove {
                onShiftRegionFound -= value;
            }
        }


        private byte GetPerceivedLuminance(Color c)
        {
            return (byte)(
                  (c.R * YUVWEIGTHED_R)
                + (c.G * YUVWEIGTHED_G)
                + (c.B * YUVWEIGTHED_B));
        }


        private static Color _late = Color.FromArgb(255, 119, 0);
        public Color LateShiftColor
        {
            get {
                return _late;
            }
            set {
                _late = value;
                InitializeReferenceColors();
            }
        }


        private static Color _morning = Color.FromArgb(0, 149, 255);
        public Color MorningShiftColor
        {
            get {
                return _morning;
            }
            set {
                _morning = value;
                InitializeReferenceColors();
            }
        }


        private static Color _onCall = Color.FromArgb(222, 202, 24);
        public Color OnCallColor
        {
            get {
                return _onCall;
            }
            set {
                _onCall = value;
                InitializeReferenceColors();
            }
        }


        private static Color _school = Color.FromArgb(0, 171, 26);
        public Color SchoolColor
        {
            get {
                return _school;
            }
            set {
                _school = value;
                InitializeReferenceColors();
            }
        }        
        

        private void SetToleratedRange(ShiftType type, byte color, int tolerance, int direction)
        {
            for (int j = 0; j < tolerance; j++) {
                byte b = (byte)(color + (direction * j));
                if (!(_shiftTypeColors.ContainsKey(b))) {
                    _shiftTypeColors.Add(b, type);
                }
            }
        }


        private bool IsShiftColor(byte c)
        {
            if (_colorRefs.Contains(c)) {
                return true;
            }
            else {
                return _shiftTypeColors.ContainsKey(c);
            }
        }


        public ShiftType GetShiftType(Rectangle scanRegion)
        {
            ShiftType shifts = ShiftType.Empty;
            BitmapData sourceData = null;
            try {
                sourceData = _bitmap.LockBits(
                    scanRegion,
                    ImageLockMode.ReadOnly,
                    _bitmap.PixelFormat);                

                int bpp = Bitmap.GetPixelFormatSize(_bitmap.PixelFormat) / BYTESIZE;
                int[] hist = new int[MAXRGB];
                unsafe
                {
                    byte* ptData = (byte*)sourceData.Scan0;
                    for (int y = 0; y < sourceData.Height; y++) {
                        byte *ptRow = ptData + (y * sourceData.Stride);
                        for (int x = 0; x < sourceData.Width * bpp; x += bpp) {
                            byte c = (byte)(
                                  (ptRow[x + 2] * YUVWEIGTHED_R) 
                                + (ptRow[x + 1] * YUVWEIGTHED_G) 
                                + (ptRow[x + 0] * YUVWEIGTHED_B));
                            hist[c]++;                            
                        }
                    }
                }

                int pxPortion = scanRegion.Width * scanRegion.Height / 8;
                byte[] shiftTypeKeys = _shiftTypeColors.Keys.ToArray();
                foreach (byte shiftColorKey in shiftTypeKeys) {
                    if (hist[shiftColorKey] >= pxPortion) {
                        shifts |= _shiftTypeColors[shiftColorKey];
                    }
                }
                return shifts;
            }
            finally {
                if (sourceData != null) {
                    _bitmap.UnlockBits(sourceData);
                }
            }
        }


        public Rectangle[] FindShiftBounds(Rectangle scanRegion)
        {
            List<Rectangle> results = new List<Rectangle>();
            bool shiftBounds = false;            
            BitmapData sourceData = null;

            const int PIXELPADDING = 1;

            try {
                sourceData = _bitmap.LockBits(
                    scanRegion,
                    ImageLockMode.ReadOnly,
                    _bitmap.PixelFormat);

                int bpp = Bitmap.GetPixelFormatSize(_bitmap.PixelFormat) / BYTESIZE;

                unsafe {
                    int xstart  = -1;
                    int xend    = -1;
                    byte* ptData = (byte*)sourceData.Scan0;
                    for (int y = 0; y < sourceData.Height; y++) {
                        byte *ptRow = ptData + (y * sourceData.Stride);
                        for (int x = 0; x < sourceData.Width * bpp; x += bpp) 
                        {
                            byte c = (byte)(
                                  (ptRow[x + 2] * YUVWEIGTHED_R)
                                + (ptRow[x + 1] * YUVWEIGTHED_G)
                                + (ptRow[x + 0] * YUVWEIGTHED_B));

                            if (IsShiftColor(c)) {
                                if (!(shiftBounds)) {
                                    shiftBounds = true;
                                    xstart = x / bpp;
                                }
                            }
                            else {
                                if (shiftBounds) {
                                    shiftBounds = false;
                                    // Subtract one pixel to exclude the
                                    // bounds that are currently included.
                                    xend = (x / bpp) - PIXELPADDING;
                                    results.Add(new Rectangle(
                                        xstart,
                                        scanRegion.Y,
                                        xend - xstart,
                                        scanRegion.Height));
                                }
                            }
                        }
                    }
                }
                return results.ToArray();
            }            
            finally {
                if (sourceData != null) {
                    _bitmap.UnlockBits(sourceData);
                }
            }            
        }


        public string GetTextFromRoi(Rectangle roi)
        {
            try {
                Rect rc = new Rect(roi.X, roi.Y, roi.Width, roi.Height);
                using (var page = _engine.Process(_pix, rc, PageSegMode.SingleLine)) {
                    return page.GetText();
                }
            }
            catch {
                return string.Empty;
            }
        }


        public Rectangle[] GetLineBasedROIs()
        {
            List<Rectangle> rois = null;

            try {    
                using (var bin = _pix.BinarizeOtsuAdaptiveThreshold(
                    _pix.Width, _pix.Height, 8, 8, 0.0f)) {
                    using (var page = _engine.Process(bin, PageSegMode.SparseText)) {
                        rois = page.GetSegmentedRegions(PageIteratorLevel.TextLine);
                    }
                }
            }
            catch {
                return new Rectangle[0];
            }
            return rois?.ToArray() ?? new Rectangle[0];
        }


        private void InitializeReferenceColors()
        {
            _lumaMorning = GetPerceivedLuminance(_morning);
            _lumaLate   = GetPerceivedLuminance(_late);
            _lumaOnCall = GetPerceivedLuminance(_onCall);
            _lumaSchool = GetPerceivedLuminance(_school);

            _colorRefs = new byte[] {
                _lumaMorning,
                _lumaLate,
                _lumaOnCall,
                _lumaSchool,
            };

            _shiftTypeColors = new Dictionary<byte, ShiftType>();

            SetToleratedRange(ShiftType.Morning, _lumaMorning, DEFAULT_TOLERANCE, DIRECTION_UP);
            SetToleratedRange(ShiftType.Morning, _lumaMorning, DEFAULT_TOLERANCE, DIRECTION_DOWN);

            SetToleratedRange(ShiftType.Late, _lumaLate, DEFAULT_TOLERANCE, DIRECTION_UP);
            SetToleratedRange(ShiftType.Late, _lumaLate, DEFAULT_TOLERANCE, DIRECTION_DOWN);

            SetToleratedRange(ShiftType.OnCall, _lumaOnCall, DEFAULT_TOLERANCE, DIRECTION_UP);
            SetToleratedRange(ShiftType.OnCall, _lumaOnCall, DEFAULT_TOLERANCE, DIRECTION_DOWN);

            SetToleratedRange(ShiftType.School, _lumaSchool, DEFAULT_TOLERANCE, DIRECTION_UP);
            SetToleratedRange(ShiftType.School, _lumaSchool, DEFAULT_TOLERANCE, DIRECTION_DOWN);            
        }


        Rectangle[] GetValidShiftRects(Rectangle[] rects, int offsetx)
        {
            List<Rectangle> results = new List<Rectangle>();
            foreach (var rect in rects) {
                if (rect.Width >= MIN_WIDTHTHRESHOLD && rect.X > offsetx) {
                    results.Add(new Rectangle(
                        rect.X, 
                        0, 
                        rect.Width,
                        rect.Height));
                }
            }
            return results.ToArray();
        }


        public Dictionary<Rectangle, string> GetRoisAndText()
        {
            var lineRois = this.GetLineBasedROIs();

            Dictionary<Rectangle, string> roiMap =
                new Dictionary<Rectangle, string>();

            for (int i = 0; i < lineRois.Length; i++) {
                lineRois[i] = ProcessorUtils.ApplyRectPadding(lineRois[i]);
                roiMap.Add(lineRois[i], this.GetTextFromRoi(lineRois[i]));
            }
            return roiMap;
        }


        private void NormalizeRoiDictionary(ref Dictionary<Rectangle, string> rois)
        {
            foreach (var rcRoi in rois.Keys.ToArray()) 
            {
                string displayText = rois[rcRoi].TrimEnd().TrimStart();
                if (string.IsNullOrEmpty(displayText)) {
                    rois.Remove(rcRoi);
                }

                if (ProcessorUtils.IsTooShort(displayText)) {
                    rois.Remove(rcRoi);
                }

                if (ProcessorUtils.HasTooManySpaces(displayText)) {
                    rois.Remove(rcRoi);
                }

                if (ProcessorUtils.TooManyInvalidChars(displayText)) {
                    rois.Remove(rcRoi);
                }
            }
        }


        private Dictionary<Rectangle, string> GetPotentialNameRois(Dictionary<Rectangle, string> rois)
        {
            Dictionary<Rectangle, string> result = new Dictionary<Rectangle, string>();
            foreach (var k in rois.Keys)
            {
                if (rois[k].Contains(',') 
                || ProcessorUtils.ContainsPotentialYear(rois[k]))
                {
                    result.Add(k, rois[k]);
                }
            }
            return result;
        }


        private void GetVerticalIntersections(
            ref RoiDetail details,
            Point midPoint,
            Rectangle[] rectangles,
            int min,
            int max)
        {
            Point top;
            Point bottom;
            foreach (var rect in rectangles) {
                top = new Point(midPoint.X, min);
                bottom = new Point(midPoint.X, max);                
                details.P1 = top;
                details.P2 = bottom;
                if (ProcessorUtils.LineIntersectsRect(top, bottom, rect)) {
                    details.Intersections++;
                }
            }
            // Decrement by one as we count the
            // midPoint "owning" rect as well.
            details.Intersections--;
        }


        private RoiDetail[] GetDetailedRois(
            Dictionary<Rectangle, string> rois, 
            int direction, 
            int min, 
            int max)
        {
            List<RoiDetail> details = new List<RoiDetail>();
            var rects = rois.Keys.ToArray();
            foreach (var roi in rois.Keys)
            {
                Point ptMid = new Point(
                    roi.X + roi.Width / 2, 
                    roi.Y + roi.Height / 2);

                RoiDetail detail = new RoiDetail() {
                    Bounds = roi,
                    MidPoint = ptMid,
                    Text = rois[roi]
                };
                
                this.GetVerticalIntersections(
                    ref detail, 
                    ptMid, rects, 
                    min, max);

                details.Add(detail);
            }
            return details.ToArray();
        }


        

        private void ApplyImportance(ref RoiDetail[] details)
        {
            int maxIntersections = details.Max(d => d.Intersections);
            int minIntersections = details.Min(d => d.Intersections);

            for (int i = 0; i < details.Length; i++)
            {
                var possibleMonth = ProcessorUtils.GetPossibleMonths(
                    details[i].Text).FirstOrDefault();

                if (!(string.IsNullOrEmpty(possibleMonth))) {
                    details[i].Importance = 3;
                    details[i].ProbablyMonth = true;
                    details[i].Text = possibleMonth;
                }

                if (details[i].Intersections == maxIntersections) {
                    details[i].Importance += (int)ImportanceIncrement.High;
                }
                else if (details[i].Intersections >= maxIntersections / 2) {
                    details[i].Importance += (int)ImportanceIncrement.Normal;
                }

                if (_numRegex.IsMatch(details[i].Text)) {
                    details[i].Importance += (int)ImportanceIncrement.High;
                }
                else {
                    if (details[i].Text.Contains(",")) {
                        details[i].Importance += (int)ImportanceIncrement.Normal;
                    }
                }
            }
        }


        private RoiDetail[] GetRoiDetails()
        {
            var rois = this.GetRoisAndText();
            this.NormalizeRoiDictionary(ref rois);

            var namedRois = this.GetPotentialNameRois(rois);
            var detailedRois = this.GetDetailedRois(
                namedRois, 0, 0, _bitmap.Height);

            this.ApplyImportance(ref detailedRois);

            return detailedRois;
        }


        private string TryGetMonth(RoiDetail[] detailedRois)
        {
            for (int i = 0; i < detailedRois.Length; i++) {
                if (detailedRois[i].ProbablyMonth) {
                    onMonthRegionFound?.Invoke(this, new OcrRegionArgs(detailedRois[i]));
                    return detailedRois[i].Text.Trim();
                }
            }
            return string.Empty;
        }


        private Rectangle[] GetShiftBounds(RoiDetail[] detailedRois)
        {
            List<Rectangle> shiftBounds = new List<Rectangle>();

            foreach (RoiDetail roiDetail in detailedRois)
            {
                if (roiDetail.Importance <= (int)Importance.Low) {
                    continue;
                }

                if (roiDetail.ProbablyMonth) {
                    continue;
                }

                Rectangle scanRegion = new Rectangle(
                    0,
                    roiDetail.Bounds.Y,
                    _bitmap.Width,
                    roiDetail.Bounds.Height);

                Rectangle[] bounds = this.FindShiftBounds(scanRegion);
                Rectangle[] validBounds = this.GetValidShiftRects(
                    bounds, roiDetail.Bounds.X);

                shiftBounds.AddRange(validBounds);

                onPersonRegionFound?.Invoke(this, new OcrRegionArgs(roiDetail));
            }
            return shiftBounds.ToArray();
        }


        private Rectangle[] GetUniqueBoundingRects(Rectangle[] shiftBounds)
        {
            Dictionary<int, Rectangle> unique = new Dictionary<int, Rectangle>();
            foreach (var rc in shiftBounds) {
                if (!(unique.ContainsKey(rc.X))) {
                    var rcUnique = new Rectangle(rc.X, 0, rc.Width, rc.Height);
                    unique.Add(rcUnique.X, rcUnique);
                }
            }

            var uniqueRects = unique.OrderBy(e => e.Key)
                .Select(e => e.Value)
                .ToArray();

            return uniqueRects;
        }


        private ShiftDayInfo[] GetShiftInfos(RoiDetail detailedRoi, Rectangle[] boundingRects)
        {
            var resultList = new List<ShiftDayInfo>();
            for (int i = 0; i < boundingRects.Length; i++)
            {
                var rc = new Rectangle(
                    boundingRects[i].X,
                    detailedRoi.Bounds.Y, boundingRects[i].Width,
                    detailedRoi.Bounds.Height);
                var shift = this.GetShiftType(rc);
                resultList.Add(new ShiftDayInfo(i + 1, shift));
                onShiftRegionFound?.Invoke(this, new OcrRegionArgs(shift.ToString(), rc));
            }
            return resultList.ToArray();
        }


        public OcrResult StartProcessing()
        {
            RoiDetail[] detailedRois = GetRoiDetails();
            if (detailedRois.Length < 1) {
                return default(OcrResult);
            }

            OcrResult ocrResult = new OcrResult();
            ocrResult.ShiftInfos = new Dictionary<string, ShiftDayInfo[]>();

            ocrResult.Month = TryGetMonth(detailedRois);

            Rectangle[] shiftBounds;            
            shiftBounds = GetShiftBounds(detailedRois);

            var uniqueBounding = this.GetUniqueBoundingRects(shiftBounds.ToArray());
            shiftBounds = null;

            foreach (var roi in detailedRois) {
                if (roi.ProbablyMonth || roi.Importance <= (int)Importance.Low) {
                    continue;
                }
                var results = GetShiftInfos(roi, uniqueBounding);
                ocrResult.ShiftInfos.Add(roi.Text.Trim(), results);
            }
            return ocrResult;
        }


        public ShiftProcessor(Bitmap source)
        {
            _engine = new TesseractEngine(
                PATH_TESSDATA, 
                DEFAULT_LANGUAGE,
                EngineMode.LstmOnly);

            _pix = PixConverter.ToPix(source);
            _pix = _pix.ConvertTo8(1);
            _bitmap = new Bitmap(source);

            InitializeReferenceColors();
        }


        ~ShiftProcessor() { Dispose(false); }
        public void Dispose() { Dispose(true); }
        private void Dispose(bool disposing)
        {
            if (disposing) {
                GC.SuppressFinalize(this);
            }
            _pix?.Dispose();
            _bitmap?.Dispose();
        }
    }
}
