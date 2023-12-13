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

        private const string LETTERS = "abcdefghijklmnopqrstuvwxyz";
        private const string DIGITS = "0123456789";

        private const int RECTPADDING = 8;
        private const int RECTPADDINGHALF = RECTPADDING / 2;

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
            foreach (string part in parts)
            {
                for (int i = 0; i < MONTHS.Length; i++)
                {
                    int distance = DamerauLevenshtein.GetDistance(part, MONTHS[i]);
                    if (distance == 1)
                    {
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


        public static void NormalizeRoiDictionary(ref Dictionary<Rectangle, string> rois)
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


        public static Rectangle[] GetUniqueBoundingRects(Rectangle[] shiftBounds)
        {
            Dictionary<int, Rectangle> unique = new Dictionary<int, Rectangle>();
            foreach (var rc in shiftBounds)
            {
                if (!(unique.ContainsKey(rc.X)))
                {
                    unique.Add(rc.X, new Rectangle(rc.X, 0, rc.Width, rc.Height));
                }
            }
            var uniqueRects = unique.OrderBy(e => e.Key).Select(e => e.Value).ToArray();
            return uniqueRects;
        }


        public static Dictionary<Rectangle, string> GetPotentialNameRois(Dictionary<Rectangle, string> rois)
        {
            Dictionary<Rectangle, string> result = new Dictionary<Rectangle, string>();
            foreach (var k in rois.Keys) {
                if (rois[k].Contains(',') 
                    || ContainsPotentialYear(rois[k]))
                {
                    result.Add(k, rois[k]);
                }
            }
            return result;
        }


        public static Dictionary<Rectangle, string> GetRoisAndText(
            ref ShiftProcessor ocrProcessor)
        {
            var lineRois = ocrProcessor.GetLineBasedROIs();
            Dictionary<Rectangle, string> roiMap = 
                new Dictionary<Rectangle, string>();

            for (int i = 0; i < lineRois.Length; i++) {
                lineRois[i] = ProcessorUtils.ApplyRectPadding(lineRois[i]);
                roiMap.Add(lineRois[i], ocrProcessor.GetTextFromRoi(lineRois[i]));
            }
            return roiMap;
        }


        public static void GetIntersections(
            ref RoiDetail details,
            Point midPoint,
            Rectangle[] rectangles,
            int direction,
            int min,
            int max)
        {
            Point top;
            Point bottom;
            foreach (var rect in rectangles) {
                // up-down
                if (direction == 0) {
                    top = new Point(midPoint.X, min);
                    bottom = new Point(midPoint.X, max);
                }
                // left-right
                else {
                    top = new Point(min, midPoint.Y);
                    bottom = new Point(max, midPoint.Y);
                }

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


        public static RoiDetail[] GetDetailedRois(
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

                RoiDetail detail = new RoiDetail()
                {
                    Bounds = roi,
                    MidPoint = ptMid,
                    Text = rois[roi]
                };
                
                GetIntersections(
                    ref detail, 
                    ptMid, 
                    rects, 
                    direction, 
                    min, 
                    max);

                details.Add(detail);
            }
            return details.ToArray();
        }


        public static void ApplyImportance(ref RoiDetail[] details)
        {
            int maxIntersections = details.Max(d => d.Intersections);
            int minIntersections = details.Min(d => d.Intersections);

            string rgxNumPattern = @"\((\d{1})+\)";
            Regex rgx = new Regex(rgxNumPattern, RegexOptions.Compiled);

            for (int i = 0; i < details.Length; i++)
            {
                var possibleMonth = ProcessorUtils.GetPossibleMonths(details[i].Text).FirstOrDefault();
                if (!(string.IsNullOrEmpty(possibleMonth))) {
                    details[i].Importance = 3;
                    details[i].ProbablyMonth = true;
                }

                if (details[i].Intersections == maxIntersections) {
                    details[i].Importance += 2;
                }
                else if (details[i].Intersections >= maxIntersections / 2) {
                    details[i].Importance += 1;
                }

                if (rgx.IsMatch(details[i].Text)) {
                    details[i].Importance += 2;
                }
                else {
                    if (details[i].Text.Contains(",")) {
                        details[i].Importance += 1;
                    }
                }
            }
        }
    }
}
