using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

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

        public static unsafe Bitmap GetTransparent(in Image image, byte opacity)
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

        public static unsafe int AddAlphas(in Image image)
        {
            int alphas = 0;
            Bitmap img = new Bitmap(image);
            BitmapData imageData = img.LockBits(new Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            byte* row = (byte*)imageData.Scan0.ToPointer();
            int bpp = 4;
            int stride = imageData.Stride;
            for (int y = 0; y < image.Height; y++)
            {
                int bi = 0, gi = 1, ri = 2, ai = 3;
                for (int x = 0; x < image.Width; x++)
                {
                    alphas += row[ai];
                    ai += bpp;
                    bi += bpp;
                    gi += bpp;
                    ri += bpp;
                }
                row += stride;
            }
            img.UnlockBits(imageData);
            img.Dispose();
            return alphas;
        }
    }

    class Layer
    {
        private Image image;

        public string name;
        public int left;
        public int top;
        public byte opacity;
        public int layerId;
        public Image Image
        {
            get
            {
                return image;
            }
            set
            {
                if(image != null)
                    image.Dispose();
                image = value;
            }
        }
        ~Layer()
        {
            if (Image != null)
                Image.Dispose();
        }
    }

    class GroupLayer
    {
        public string name;
        public int groupLayerId;
        public List<Layer> layers;
        ~GroupLayer()
        {
            layers.Clear();
        }
    }
}

