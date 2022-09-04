using System.Drawing;

namespace Kyrsova
{
    static class ImageProps
    {
        public static string GetImageFormat(string Filename)
        {
            int DotIndex = Filename.IndexOf(".");
            return Filename.Substring(DotIndex);
        }

        public static int GetBpp(Image image) //Bit per pixel
        {
            string PixelFormat = image.PixelFormat.ToString();
            if (PixelFormat.Contains("Format") && PixelFormat.Contains("bpp"))
            {
                string Bpp = PixelFormat.Substring(6, PixelFormat.IndexOf("bpp") - 6);
                int BppInt = 0;
                if (int.TryParse(Bpp, out BppInt))
                    return BppInt;
                else
                    return 0;
            }
            else
                return 0;
        }

        public static bool GetAlpha(Image image)
        {
            Bitmap img = (Bitmap)image;
            Color color = img.GetPixel(0, 0);
            return color.A != 255;
        }
    }
}
