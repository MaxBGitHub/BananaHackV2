using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BananaHackV2.OCR
{
    internal static class ProcessorUtils
    {
        private const string RGX_PATTERNDATE = @"\d{4}";
        private static readonly Regex _regexDate = new Regex(RGX_PATTERNDATE, RegexOptions.Compiled);        

        private const string LETTERS    = "abcdefghijklmnopqrstuvwxyz";
        private const string DIGITS     = "0123456789";

        private const int RECTPADDING       = 8;
        private const int RECTPADDINGHALF   = RECTPADDING / 2;

        public static readonly string[] MONTHS = {
            "januar",   "februar",  "märz",
            "april",    "mai",      "juni",
            "juli",     "august",   "september",
            "oktober",  "november", "dezember"
        };

        public static readonly Func<Rectangle, Rectangle> GetPaddedRect = (rc) => {
            return new Rectangle(
                rc.X - RECTPADDINGHALF,
                rc.X - RECTPADDINGHALF,
                rc.Width + RECTPADDING,
                rc.Height + RECTPADDING
            );
        };


        public static IEnumerable<string> GetPossibleMonths(string input)
        {
            string[] parts = input.Split('\u0020');
            foreach (string part in parts) {
                for (int i = 0; i < MONTHS.Length; i++) {
                    int distance = DamerauLevenshtein.GetDistance(part, MONTHS[i]);
                    if (distance == 1) {
                        yield return part;
                    }
                }
            }
        }


        public static readonly Func<string, bool> HasTooManySpaces = (s) => {
            return (s.Where(c => c == '\u0020').Count() >= (s.Length / 2) - 1);
        };


        public static readonly Func<string, bool> IsTooShort = (s) => {
            return s.Length < 3;
        };


        public static readonly Func<string, bool> TooManyInvalidChars = (s) => {
            int letterCount = s.ToLower().Where(c => LETTERS.IndexOf(c) >= 0).Count();
            int digitCount = s.ToLower().Where(c => DIGITS.IndexOf(c) >= 0).Count();
            return (letterCount + digitCount <= s.Replace('\u0020', '\u0000').Length / 2);
        };


        public static readonly Func<string, bool> ContainsPotentialYear = (s) =>
        {
            return _regexDate.IsMatch(s);
        };


        public static readonly Func<Rectangle, Rectangle> ApplyRectPadding = (roi) =>
        {            
            roi.X -= RECTPADDINGHALF;
            roi.Y -= RECTPADDINGHALF;
            roi.Width += RECTPADDING;
            roi.Height += RECTPADDING;

            return roi;
        };


        public static bool LineIntersectsRect(Point p1, Point p2, Rectangle r)
        {
            return LineIntersectsLine(p1, p2, new Point(r.X, r.Y), new Point(r.X + r.Width, r.Y)) ||
                   LineIntersectsLine(p1, p2, new Point(r.X + r.Width, r.Y), new Point(r.X + r.Width, r.Y + r.Height)) ||
                   LineIntersectsLine(p1, p2, new Point(r.X + r.Width, r.Y + r.Height), new Point(r.X, r.Y + r.Height)) ||
                   LineIntersectsLine(p1, p2, new Point(r.X, r.Y + r.Height), new Point(r.X, r.Y)) ||
                   (r.Contains(p1) && r.Contains(p2));
        }

        public static bool LineIntersectsLine(Point l1p1, Point l1p2, Point l2p1, Point l2p2)
        {
            float q = (l1p1.Y - l2p1.Y) * (l2p2.X - l2p1.X) - (l1p1.X - l2p1.X) * (l2p2.Y - l2p1.Y);
            float d = (l1p2.X - l1p1.X) * (l2p2.Y - l2p1.Y) - (l1p2.Y - l1p1.Y) * (l2p2.X - l2p1.X);

            if (d == 0) {
                return false;
            }

            float r = q / d;
            q = (l1p1.Y - l2p1.Y) * (l1p2.X - l1p1.X) - (l1p1.X - l2p1.X) * (l1p2.Y - l1p1.Y);
            float s = q / d;

            if (r < 0 || r > 1 || s < 0 || s > 1) {
                return false;
            }

            return true;
        }

    }
}
