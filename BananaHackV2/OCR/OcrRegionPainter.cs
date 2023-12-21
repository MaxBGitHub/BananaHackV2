using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BananaHackV2.OCR
{
    internal class OcrRegionPainter : IDisposable
    {
        private OCR.ShiftProcessor _processor;
        private Graphics _grCanvas;

        private Pen _shiftPen = new Pen(Color.White, 1f);
        private SolidBrush _shiftBrush = new SolidBrush(Color.White);

        private Color _shiftColor = Color.White;
        public Color ShiftBoxColor
        {
            get {
                return _shiftColor;
            }
            set {
                if (value != _shiftColor) {
                    _shiftColor = value;
                    _shiftPen?.Dispose();
                    _shiftPen = new Pen(_shiftColor, 2f);
                    _shiftBrush?.Dispose();
                    _shiftBrush = new SolidBrush(_shiftColor);
                }
            }
        }


        private Pen _namePen = new Pen(Color.Red, 2f);
        private SolidBrush _nameBrush = new SolidBrush(Color.Red);

        private Color _nameColor = Color.Red;
        public Color NameBoxColor
        {
            get {
                return _nameColor;
            }
            set {
                if (value != _nameColor) {
                    _nameColor = value;
                    _namePen?.Dispose();
                    _namePen = new Pen(_nameColor, 2f);
                    _nameBrush?.Dispose();
                    _nameBrush = new SolidBrush(_nameColor);
                }
            }
        }


        private Pen _monthPen = new Pen(Color.Green, 2f);
        private SolidBrush _monthBrush = new SolidBrush(Color.Green);

        private Color _monthColor = Color.Green;
        public Color MonthBoxColor
        {
            get {
                return _monthColor;
            }
            set {
                if (value != _monthColor) {
                    _nameColor = value;
                    _monthPen?.Dispose();
                    _monthPen = new Pen(_monthColor, 2f);
                    _monthBrush?.Dispose();
                    _monthBrush = new SolidBrush(_monthColor);
                }
            }
        }

        private Font _font = new Font("Consolas", 16f, FontStyle.Regular, GraphicsUnit.Pixel);
        public Font Font
        {
            get {
                return _font;
            }
            set {
                if (value != _font) {
                    _font?.Dispose();
                    _font = value;
                }
            }
        }


        private Font _shiftFont = new Font("Consolas", 8f, FontStyle.Regular, GraphicsUnit.Pixel);
        public Font ShiftFont
        {
            get {
                return _shiftFont;
            }
            set {
                if (value != _shiftFont) {
                    _shiftFont?.Dispose();
                    _shiftFont = value;
                }
            }
        }


        private Rectangle GetTextBounds(string text, Rectangle bounds)
        {
            SizeF szText = _grCanvas.MeasureString(text, _font);
            int textWidth = (int)Math.Ceiling(szText.Width);
            int textHeight = (int)Math.Ceiling(szText.Height);

            Rectangle rcTextBounds = new Rectangle(
                bounds.Right,
                bounds.Top - textHeight / 2,
                textWidth,
                textHeight);

            return rcTextBounds;
        }


        private void OnPersonRegionFound(object sender, OcrRegionArgs e)
        {
            var textBounds = GetTextBounds(e.Text, e.Bounds);

            _grCanvas.DrawRectangle(_namePen, e.Bounds);
            _grCanvas.DrawString(e.Text, _font, _nameBrush, textBounds);
        }


        private void OnMonthRegionFound(object sender, OcrRegionArgs e)
        {
            var textBounds = GetTextBounds(e.Text, e.Bounds);

            _grCanvas.DrawRectangle(_monthPen, e.Bounds);
            _grCanvas.DrawString(e.Text, _font, _monthBrush, textBounds);
        }


        private void OnShiftRegionFound(object sender, OcrRegionArgs e)
        {
            _grCanvas.DrawRectangle(_shiftPen, e.Bounds);
            _grCanvas.DrawString(e.Text, _shiftFont, _shiftBrush, e.Bounds);
        }


        public OcrRegionPainter(ShiftProcessor processor, ref Graphics gr)
        {
            _grCanvas = gr;
            _processor = processor;
            _processor.MonthRegionFound += OnMonthRegionFound;
            _processor.ShiftRegionFound += OnShiftRegionFound;
            _processor.PersonRegionFound += OnPersonRegionFound;
        }


        private void FlushGdiObjects()
        {
            _font?.Dispose();
            _shiftFont?.Dispose();
            _shiftBrush?.Dispose();
            _shiftPen?.Dispose();
            _monthBrush?.Dispose();
            _monthPen?.Dispose();
            _nameBrush?.Dispose();
            _namePen?.Dispose();
        }


        ~OcrRegionPainter() { Dispose(false); }
        public void Dispose() { Dispose(true); }
        private void Dispose(bool disposing)
        {
            if (disposing) {
                GC.SuppressFinalize(this);
            }
            FlushGdiObjects();
        }
    }
}
