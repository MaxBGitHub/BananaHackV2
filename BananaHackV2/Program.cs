using BananaHackV2.OCR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace BananaHackV2
{    
    internal class Program
    {        
        private static void PerformOcr(string imagePath)
        {
            var sp = Stopwatch.StartNew();

            Bitmap source = (Bitmap)Image.FromFile(imagePath);
            var processor = new ShiftProcessor(source);

            var rois = ProcessorUtils.GetRoisAndText(ref processor);
            ProcessorUtils.NormalizeRoiDictionary(ref rois);

            var nameRois = ProcessorUtils.GetPotentialNameRois(rois);
            var detailedRois = ProcessorUtils.GetDetailedRois(nameRois, 0, 0, source.Height);
            ProcessorUtils.ApplyImportance(ref detailedRois);

            Pen namePen  = new Pen(Brushes.Red, 2f);
            Pen monthPen = new Pen(Brushes.Green, 2f);

            Font f = new Font("Consolas", 16f, FontStyle.Regular, GraphicsUnit.Pixel);

            Pen temp = new Pen(Color.FromArgb(60, Color.Aquamarine.R, Color.Aquamarine.G, Color.Aquamarine.B));

            List<Rectangle> shiftBounds = new List<Rectangle>();

            using (var g = Graphics.FromImage(source)) 
            {                
                foreach (var roi in detailedRois)
                {
                    if (roi.Importance < 3) {
                        continue;
                    }
                    string displayText = roi.Text;

                    //Brush brush = null;
                    if (roi.ProbablyMonth) {
                        g.DrawRectangle(monthPen, roi.Bounds);
                    }
                    else {
                        g.DrawRectangle(namePen, roi.Bounds);
                        g.DrawLine(Pens.GreenYellow, new Point(0, roi.MidPoint.Y), new Point(source.Width, roi.MidPoint.Y));
                        Rectangle scanRegion = new Rectangle(0, roi.Bounds.Y, source.Width, roi.Bounds.Height);
                        var bounds = processor.FindShiftBounds(scanRegion);
                        foreach (var rect in bounds) {
                            if (rect.Width >= 12 && rect.X > roi.Bounds.X) {
                                shiftBounds.Add(new Rectangle(rect.X, 0, rect.Width, rect.Height));
                            }
                        }
                    }
                    var sz = g.MeasureString(displayText, f);
                    int tW = (int)Math.Ceiling(sz.Width);
                    int tH = (int)Math.Ceiling(sz.Height);
                    var rcText = new Rectangle(roi.Bounds.Right, roi.Bounds.Top - tH / 2, tW, tH);
                    g.DrawString(displayText, f, Brushes.Yellow, rcText);
                    g.FillEllipse(Brushes.DarkOrange, roi.MidPoint.X - 3, roi.MidPoint.Y - 3, 6, 6);
                }

                var padded = ProcessorUtils.GetUniqueBoundingRects(shiftBounds.ToArray());
                foreach (var roi in detailedRois)
                {
                    if (roi.ProbablyMonth || roi.Importance < 3) {
                        continue;
                    }

                    foreach (var rect in padded)
                    {
                        var rc = new Rectangle(rect.X, roi.Bounds.Y, rect.Width, roi.Bounds.Height);
                        var shift = processor.GetShiftType(rc);
                        g.DrawRectangle(Pens.Aquamarine, new Rectangle(rect.X, roi.Bounds.Y, rect.Width, roi.Bounds.Height));
                        g.DrawString(shift.ToString(), new Font(f.FontFamily, 8.0f, FontStyle.Bold, GraphicsUnit.Pixel), Brushes.White, rc);
                    }
                }
            }

            namePen.Dispose();
            monthPen.Dispose();
            processor.Dispose();            

            sp.Stop();
            Console.WriteLine(sp.Elapsed);

            string newName = "OCR_" + Path.GetFileName(imagePath);
            string name = Path.GetFileName(imagePath);
            string newPath = imagePath.Replace(name, newName);
            source.Save(
                newPath, 
                System.Drawing.Imaging.ImageFormat.Png);

            source.Dispose();
        }


        [STAThread]
        static void Main(string[] args)
        {
            string folder = null;
            using (var ofd = new System.Windows.Forms.FolderBrowserDialog()) {
                ofd.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                    folder = ofd.SelectedPath;
                }
            }

            if (string.IsNullOrEmpty(folder)) {
                return;
            }

            foreach (var file in Directory.GetFiles(folder)) {
                PerformOcr(file);
            }            
            Console.WriteLine("Done");
        }
    }
}
