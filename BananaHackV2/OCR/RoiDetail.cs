using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BananaHackV2.OCR
{
    public struct RoiDetail
    {
        public Rectangle Bounds;
        public Point MidPoint;
        public string Text;
        public int Intersections;
        public Point P1;
        public Point P2;
        public int Importance;
        public bool ProbablyMonth;
    }
}
