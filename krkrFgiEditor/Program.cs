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

    public class Layer
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

        public static bool HasNone(List<Layer> layers)
        {
            foreach (Layer layer in layers)
                if (layer.Image==null)
                    return true;
            return false;
        }

        public static bool IsAllNone(List<Layer> layers)
        {
            foreach(Layer layer in layers)
                if(layer.Image != null)
                    return false;
            return true;
        }

        public static Image GenerateImage(List<Layer> layers)
        {
            if (layers.Count == 0 || IsAllNone(layers))
                return null;
            int left = int.MaxValue, right = 0, top = int.MaxValue, bottom = 0;
            foreach (Layer layer in layers)
            {
                if(layer.Image == null)
                    continue;
                int layerRight = layer.left + layer.Image.Width;
                int layerBottom = layer.top + layer.Image.Height;
                if (layer.left < left)
                    left = layer.left;
                if (layerRight > right)
                    right = layerRight;
                if (layer.top < top)
                    top = layer.top;
                if (layerBottom > bottom)
                    bottom = layerBottom;
            }
            int width = right - left;
            int height = bottom - top;
            Image image = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(image);
            foreach (Layer layer in layers)
                if (layer.Image != null)
                    if (layer.opacity == 255)
                        g.DrawImage(layer.Image, layer.left - left, layer.top - top);
                    else
                        g.DrawImage(Program.GetTransparent(layer.Image, layer.opacity),
                            layer.left - left, layer.top - top);
            g.Dispose();
            return image;
        }
    }

    public class GroupLayer
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

