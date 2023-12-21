using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BananaHackV2.OCR
{
    internal class OcrRegionArgs : EventArgs
    {
        public readonly string Text;
        public readonly Rectangle Bounds;

        public OcrRegionArgs(RoiDetail roiDetails)
        {
            Text = roiDetails.Text.Trim();
            Bounds = roiDetails.Bounds;
        }

        public OcrRegionArgs(string text, Rectangle bounds)
        {
            Text = text;
            Bounds = bounds;
        }
    }
}
