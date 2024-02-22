using System;
using System.Windows.Media.Imaging;
using System.IO;
using ImageMagick;
using System.Collections;

namespace WebPBGone
{
    internal class Program
    {
        static void Main(string[] args)
        {

        }

        static void convertToPng(string filePath)
        {
            var outputFile = Path.ChangeExtension(filePath, "png");
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                using (var image = new MagickImage(stream))
                {
                    using (var outputStream = new FileStream(filePath, FileMode.Create))
                    {
                        image.Format = MagickFormat.Png;
                        image.Quality = 80;
                        image.Write(outputStream);
                    }
                }
            }
        }
    }
}
