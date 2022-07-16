using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace krkrFgiEditor
{
    internal static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainWin());
        }

        public static Image ResizeImage(Image image, int width, int height)
        {
            float widthRatio = (float)image.Width / width;
            float heightRatio = (float)image.Height / height;
            int newWidth, newHeight;
            if (widthRatio < heightRatio)
            {
                newWidth = (int)(image.Width / heightRatio);
                newHeight = height;
            }
            else
            {
                newWidth = width;
                newHeight = (int)(image.Height / widthRatio);
            }
            return image.GetThumbnailImage(newWidth, newHeight, () => false, IntPtr.Zero);
        }

        public unsafe static Bitmap GetTransparent(in Image image,byte opacity)
        {
            Bitmap result = new Bitmap(image);
            BitmapData resultData = result.LockBits(new Rectangle(0, 0, result.Width, result.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            byte* row = (byte*)resultData.Scan0.ToPointer();
            int bpp = 4;
            int stride = resultData.Stride;
            for (int y = 0; y < result.Height; y++)
            {
                int bi = 0, gi = 1, ri = 2, ai = 3;
                for (int x = 0; x < result.Width; x++)
                {
                    if (row[ai] != 0)
                        row[ai] = (byte)(row[ai] * (double)opacity / 255);
                    ai += bpp;
                    bi += bpp;
                    gi += bpp;
                    ri += bpp;
                }
                row += stride;
            }
            result.UnlockBits(resultData);
            return result;
        }
    }

    class Layer
    {
        public string name;
        public int left;
        public int top;
        public byte opacity;
        public int layerId;
        public Image image;
    }

    class GroupLayer
    {
        public string name;
        public int groupLayerId;
        public List<Layer> layers;
    }
}

