using System;
using System.Drawing;
using System.Drawing.Imaging;


namespace BananaHackV2.ocr
{
    public static class Otsu
    {
        private const double YUVWEIGTHED_R = 0.299;
        private const double YUVWEIGTHED_G = 0.587;
        private const double YUVWEIGTHED_B = 0.114;

        private const int MAX_RANGE = 256;
        

        private static readonly Func<byte, byte, byte, double> CalcYUV = (r, g, b) => {
            double avg = (
                  (r * YUVWEIGTHED_R)
                + (g * YUVWEIGTHED_G)
                + (b * YUVWEIGTHED_B)
            );
            return avg;
        };


        private static unsafe void SetPixelRgb(byte* ptPixel, int index, byte value) 
        {
            ptPixel[index + 2] = value;
            ptPixel[index + 1] = value;
            ptPixel[index + 0] = value;
        }

        private static unsafe void SetPixelArgb(byte* ptPixel, int index, byte value)
        {
            ptPixel[index + 3] = 255;
            ptPixel[index + 2] = value;
            ptPixel[index + 1] = value;
            ptPixel[index + 0] = value;
        }


        private unsafe delegate void SetPixel(byte*  ptPixel, int index, byte value);


        private static int[] GetIntensityHistogramRgb(Bitmap source)
        {
            BitmapData bitmapData = null;
            try
            {
                bitmapData = source.LockBits(
                    new Rectangle(0, 0, source.Width, source.Height),
                    ImageLockMode.ReadOnly,
                    source.PixelFormat);

                int w = bitmapData.Width;
                int h = bitmapData.Height;
                int bpp = Bitmap.GetPixelFormatSize(bitmapData.PixelFormat) / 8;

                int[] hist = new int[MAX_RANGE];

                unsafe
                {
                    byte* ptFirst = (byte*)bitmapData.Scan0;
                    for (int y = 0; y < h; y++)
                    {
                        byte* ptPixel = ptFirst + (y * bitmapData.Stride);
                        for (int x = 0; x < bitmapData.Stride; x += bpp)
                        {
                            int intensity = (int)(
                                  (ptPixel[x + 2] * YUVWEIGTHED_R)
                                + (ptPixel[x + 1] * YUVWEIGTHED_B)
                                + (ptPixel[x    ] * YUVWEIGTHED_G));
                            hist[intensity]++;
                        }
                    }
                }
                return hist;
            }
            finally
            {
                if (bitmapData != null) {
                    source.UnlockBits(bitmapData);
                }
            }
        }


        public static int GetOtsuThreshold(int imageWidth, int imageHeight, int[] hist)
        {
            int N = imageWidth * imageHeight;
            int threshold = 0;

            float sum = 0;
            float sumB = 0;
            int q1 = 0;
            int q2 = 0;
            float varMax = 0;

            for (int i = 0; i <= byte.MaxValue; i++) {
                sum += i * hist[i];
            }

            for (int i = 0; i <= byte.MaxValue; i++)
            {
                q1 += hist[i];
                if (q1 == 0) {
                    continue;
                }

                q2 = N - q1;
                if (q2 == 0) {
                    break;
                }

                sumB += (float)(i * hist[i]);
                float m1 = sumB / q1;
                float m2 = (sum - sumB) / q2;

                float varBetween = (float)q1 * (float)q2 * (m1 - m2) * (m1 - m2);
                if (varBetween > varMax) {
                    varMax = varBetween;
                    threshold = i;
                }
            }
            return threshold;
        }


        private static Bitmap ApplyOtsuThreshold8BppIndexedInternal(Bitmap source, int threshold)
        {
            BitmapData sourceData = null;
            // Destination image.
            Bitmap destination = new Bitmap(
                source.Width,
                source.Height,
                source.PixelFormat);
            destination.Palette = source.Palette;
            BitmapData destinationData = null;

            try
            {
                // Get source image data on read-only mode.
                // Pixel data will be copied to destination image.
                sourceData = source.LockBits(
                    new Rectangle(0, 0, source.Width, source.Height),
                    ImageLockMode.ReadOnly,
                    source.PixelFormat);
                // Get destionation image data on write mode to 
                // copy source data to destination.
                destinationData = destination.LockBits(
                    new Rectangle(0, 0, destination.Width, destination.Height),
                    ImageLockMode.WriteOnly,
                    destination.PixelFormat);

                int w = sourceData.Width;
                int h = sourceData.Height;

                // Byte per pixel.
                int bppSrc = Bitmap.GetPixelFormatSize(sourceData.PixelFormat) / 8;

                unsafe
                {
                    // Pointer to first scan line.
                    // Access point to image data.
                    byte* ptFirstSrc = (byte*)sourceData.Scan0;
                    byte* ptFirstDest = (byte*)destinationData.Scan0;
                    for (int y = 0; y < h; y++)
                    {
                        // Address of next image stride (row).
                        byte* ptPixelSrc = ptFirstSrc + (y * sourceData.Stride);
                        byte* ptPixelDest = ptFirstDest + (y * destinationData.Stride);
                        // Iterate over each pixel. 
                        // Skip the n-Byte per pixel (bppSrc) to skip the 
                        // included color channels. 
                        // RGB has 3 channels for example. 
                        // To reach the next pixel you need to skip these channels
                        // e.g. bpp (byte per pixel).
                        for (int x = 0; x < sourceData.Stride; x += bppSrc)
                        {
                            // Assign black or white pixel value.
                            byte val = (byte)(ptPixelSrc[x] >= threshold ? 255 : 0);
                            ptPixelDest[x] = val;
                        }
                    }
                }
            }
            finally
            {
                if (sourceData != null) {
                    source.UnlockBits(sourceData);
                }

                if (destinationData != null) {
                    destination.UnlockBits(destinationData);
                }
            }

            return destination;
        }


        private static Bitmap ApplyOtsuThresholdInternal(Bitmap source, int threshold)
        {
            BitmapData sourceData = null;
            // Destination image.
            Bitmap destination = new Bitmap(
                source.Width,
                source.Height,
                source.PixelFormat);
            BitmapData destinationData = null;            

            try
            {
                // Get source image data on read-only mode.
                // Pixel data will be copied to destination image.
                sourceData = source.LockBits(
                    new Rectangle(0, 0, source.Width, source.Height),
                    ImageLockMode.ReadOnly,
                    source.PixelFormat);
                // Get destionation image data on write mode to 
                // copy source data to destination.
                destinationData = destination.LockBits(
                    new Rectangle(0, 0, destination.Width, destination.Height),
                    ImageLockMode.WriteOnly,
                    destination.PixelFormat);

                int w = sourceData.Width;
                int h = sourceData.Height;

                // Byte per pixel.
                int bppSrc = Bitmap.GetPixelFormatSize(sourceData.PixelFormat) / 8;                

                unsafe
                {
                    SetPixel setPixel;
                    if (bppSrc == 4) {
                        setPixel = SetPixelArgb;
                    }
                    else {
                        setPixel = SetPixelRgb;
                    }

                    // Pointer to first scan line.
                    // Access point to image data.
                    byte* ptFirstSrc = (byte*)sourceData.Scan0;
                    byte* ptFirstDest = (byte*)destinationData.Scan0;
                    for (int y = 0; y < h; y++)
                    {
                        // Address of next image stride (row).
                        byte* ptPixelSrc = ptFirstSrc + (y * sourceData.Stride);
                        byte* ptPixelDest = ptFirstDest + (y * destinationData.Stride);
                        // Iterate over each pixel. 
                        // Skip the n-Byte per pixel (bppSrc) to skip the 
                        // included color channels. 
                        // RGB has 3 channels for example. 
                        // To reach the next pixel you need to skip these channels
                        // e.g. bpp (byte per pixel).
                        for (int x = 0; x < sourceData.Stride; x += bppSrc)
                        {
                            // RGB to YUV conversion which returns a grayscale 
                            // representation of the color.
                            // Used to determine destination pixel color.
                            double avg = CalcYUV(
                                ptPixelSrc[x + 2], 
                                ptPixelSrc[x + 1], 
                                ptPixelSrc[x + 0]);
                            // Assign black or white pixel value.
                            byte val = (byte)(avg >= threshold ? 255 : 0);
                            setPixel(ptPixelDest, x, val);
                            //if (bppSrc == 4) {
                            //    ptPixelDest[x+3] = 255;
                            //}
                            //ptPixelDest[x + 2] = val;
                            //ptPixelDest[x + 1] = val;
                            //ptPixelDest[x    ] = val;
                        }
                    }
                }
            }
            finally
            {
                if (sourceData != null)
                {
                    source.UnlockBits(sourceData);
                }

                if (destinationData != null)
                {
                    destination.UnlockBits(destinationData);
                }
            }

            return destination;
        }


        private static Bitmap ApplyOtsuThreshold8BppIndexedInternalInverted(Bitmap source, int threshold)
        {
            BitmapData sourceData = null;
            // Destination image.
            Bitmap destination = new Bitmap(
                source.Width,
                source.Height,
                source.PixelFormat);
            destination.Palette = source.Palette;
            BitmapData destinationData = null;

            try
            {
                // Get source image data on read-only mode.
                // Pixel data will be copied to destination image.
                sourceData = source.LockBits(
                    new Rectangle(0, 0, source.Width, source.Height),
                    ImageLockMode.ReadOnly,
                    source.PixelFormat);
                // Get destionation image data on write mode to 
                // copy source data to destination.
                destinationData = destination.LockBits(
                    new Rectangle(0, 0, destination.Width, destination.Height),
                    ImageLockMode.WriteOnly,
                    destination.PixelFormat);

                int w = sourceData.Width;
                int h = sourceData.Height;

                // Byte per pixel.
                int bppSrc = Bitmap.GetPixelFormatSize(sourceData.PixelFormat) / 8;

                unsafe
                {
                    // Pointer to first scan line.
                    // Access point to image data.
                    byte* ptFirstSrc = (byte*)sourceData.Scan0;
                    byte* ptFirstDest = (byte*)destinationData.Scan0;
                    for (int y = 0; y < h; y++)
                    {
                        // Address of next image stride (row).
                        byte* ptPixelSrc = ptFirstSrc + (y * sourceData.Stride);
                        byte* ptPixelDest = ptFirstDest + (y * destinationData.Stride);
                        // Iterate over each pixel. 
                        // Skip the n-Byte per pixel (bppSrc) to skip the 
                        // included color channels. 
                        // RGB has 3 channels for example. 
                        // To reach the next pixel you need to skip these channels
                        // e.g. bpp (byte per pixel).
                        for (int x = 0; x < sourceData.Stride; x += bppSrc) {
                            // Assign black or white pixel value.
                            byte val = (byte)(ptPixelSrc[x] >= threshold ? 0 : 255);
                            ptPixelDest[x] = val;
                        }
                    }
                }
            }
            finally
            {
                if (sourceData != null) {
                    source.UnlockBits(sourceData);
                }

                if (destinationData != null) {
                    destination.UnlockBits(destinationData);
                }
            }

            return destination;
        }


        private static Bitmap ApplyOtsuThresholdInternalInverted(Bitmap source, int threshold)
        {
            BitmapData sourceData = null;
            // Destination image.
            Bitmap destination = new Bitmap(
                source.Width,
                source.Height,
                source.PixelFormat);
            BitmapData destinationData = null;

            try
            {
                // Get source image data on read-only mode.
                // Pixel data will be copied to destination image.
                sourceData = source.LockBits(
                    new Rectangle(0, 0, source.Width, source.Height),
                    ImageLockMode.ReadOnly,
                    source.PixelFormat);
                // Get destionation image data on write mode to 
                // copy source data to destination.
                destinationData = destination.LockBits(
                    new Rectangle(0, 0, destination.Width, destination.Height),
                    ImageLockMode.WriteOnly,
                    destination.PixelFormat);

                int w = sourceData.Width;
                int h = sourceData.Height;

                // Byte per pixel.
                int bppSrc = Bitmap.GetPixelFormatSize(sourceData.PixelFormat) / 8;

                unsafe
                {
                    SetPixel setPixel;
                    if (bppSrc == 4) {
                        setPixel = SetPixelArgb;
                    }
                    else {
                        setPixel = SetPixelRgb;
                    }

                    // Pointer to first scan line.
                    // Access point to image data.
                    byte* ptFirstSrc = (byte*)sourceData.Scan0;
                    byte* ptFirstDest = (byte*)destinationData.Scan0;
                    for (int y = 0; y < h; y++) 
                    {
                        // Address of next image stride (row).
                        byte* ptPixelSrc = ptFirstSrc + (y * sourceData.Stride);
                        byte* ptPixelDest = ptFirstDest + (y * destinationData.Stride);
                        // Iterate over each pixel. 
                        // Skip the n-Byte per pixel (bppSrc) to skip the 
                        // included color channels. 
                        // RGB has 3 channels for example. 
                        // To reach the next pixel you need to skip these channels
                        // e.g. bpp (byte per pixel).
                        for (int x = 0; x < sourceData.Stride; x += bppSrc)
                        {
                            // RGB to YUV conversion which returns a grayscale 
                            // representation of the color.
                            // Used to determine destination pixel color.
                            double avg = CalcYUV(
                                ptPixelSrc[x + 2],
                                ptPixelSrc[x + 1],
                                ptPixelSrc[x + 0]);
                            // Assign black or white pixel value.
                            byte val = (byte)(avg >= threshold ? 0 : 255);
                            setPixel(ptPixelDest, x, val);
                            //if (bppSrc == 4) {
                            //    ptPixelDest[x + 3] = 255;
                            //}
                            //ptPixelDest[x + 2] = val;
                            //ptPixelDest[x + 1] = val;
                            //ptPixelDest[x    ] = val;
                        }
                    }
                }
            }
            finally
            {
                if (sourceData != null) {
                    source.UnlockBits(sourceData);
                }

                if (destinationData != null){
                    destination.UnlockBits(destinationData);
                }
            }

            return destination;
        }


        public static Bitmap GetBinarizedOtsuInverted(Bitmap source)
        {
            int[] hist = GetIntensityHistogramRgb(source);
            int otsuThreshold = GetOtsuThreshold(source.Width, source.Height, hist);
            if (source.PixelFormat != PixelFormat.Format8bppIndexed)
            {
                return ApplyOtsuThresholdInternalInverted(source, otsuThreshold);
            }
            else
            {
                return ApplyOtsuThreshold8BppIndexedInternalInverted(source, otsuThreshold);
            }
        }


        public static Bitmap GetBinarizedOtsu(Bitmap source)
        {
            int[] hist = GetIntensityHistogramRgb(source);
            int otsuThreshold = GetOtsuThreshold(source.Width, source.Height, hist);
            if (source.PixelFormat != PixelFormat.Format8bppIndexed)
            {
                return ApplyOtsuThresholdInternal(source, otsuThreshold);
            }
            else
            {
                return ApplyOtsuThreshold8BppIndexedInternal(source, otsuThreshold);
            }
        }
    }
}