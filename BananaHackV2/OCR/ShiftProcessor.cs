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

namespace BananaHackV2.OCR
{
    internal class ShiftProcessor : IDisposable
    {
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

        private const string DEFAULT_LANGUAGE = "deu";


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


        //private ShiftType TryGetShiftType(byte c)
        //{
        //    if ()
        //}


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
                using (var engine = new TesseractEngine(
                    "./tessdata",
                    DEFAULT_LANGUAGE, 
                    EngineMode.LstmOnly))
                {
                    Rect rc = new Rect(roi.X, roi.Y, roi.Width, roi.Height);
                    using (var page = engine.Process(_pix, rc, PageSegMode.SingleLine)) {
                        return page.GetText();
                    }
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
                using (var engine = new TesseractEngine(
                    "./tessdata",
                    DEFAULT_LANGUAGE, 
                    EngineMode.LstmOnly))
                {                    
                    using (var bin = _pix.BinarizeOtsuAdaptiveThreshold(
                        _pix.Width, _pix.Height, 8, 8, 0.0f)) {
                        using (var page = engine.Process(bin, PageSegMode.SparseText)) {
                            rois = page.GetSegmentedRegions(PageIteratorLevel.TextLine);
                        }
                    }
                }
            }
            catch {
                return new Rectangle[0];
            }
            return rois?.ToArray() ?? new Rectangle[0];
        }


        public List<Rectangle> GetWordBasedROIs()
        {
            List<Rectangle> rois = null;
            try {
                using (var engine = new TesseractEngine(
                    "./tessdata",
                    DEFAULT_LANGUAGE, 
                    EngineMode.LstmOnly))
                {
                    using (var bin = _pix.BinarizeOtsuAdaptiveThreshold(
                        _pix.Width, _pix.Height, 8, 8, 0.0f)) {
                        using (var page = engine.Process(bin, PageSegMode.SparseText)) {
                            rois = page.GetSegmentedRegions(PageIteratorLevel.Word);
                        }
                    }
                }
            }
            catch {
            }
            return rois;
        }


        public Bitmap GetBinarized(int smoothx, int smoothy, float scorefract)
        {
            using (var bin = _pix.BinarizeOtsuAdaptiveThreshold(
                _pix.Width, _pix.Height, smoothx, smoothy, scorefract))
            {
                return PixConverter.ToBitmap(bin);
            }
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


        public ShiftProcessor(Bitmap source)
        {
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
        }
    }
}
