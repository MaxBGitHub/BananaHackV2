using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BananaHackV2.UI
{
    public class ScreenshotEventArgs
    {
        public Bitmap       Screenshot;
        public Rectangle    Bounds;
        public Point        LocationOnScreen;

        public ScreenshotEventArgs(Bitmap screenshot, Point locationOnScreen)
        {
            this.Screenshot = screenshot;
            LocationOnScreen = locationOnScreen;
            Bounds = new Rectangle(
                locationOnScreen.X, 
                locationOnScreen.Y, 
                screenshot.Width, 
                screenshot.Height);
        }
    }
}
