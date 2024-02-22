using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ImageMagick;

namespace WebPBGone
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (var stream = new MemoryStream(byteArray))
            {
                using (var image = new MagickImage(stream))
                {
                    // Convert the WebP image to a JPG image
                    using (var outputStream = new MemoryStream())
                    {
                        image.Format = MagickFormat.Png;
                        image.Quality = 80;
                        image.Write(outputStream);

                        return Convert.ToBase64String(outputStream.ToArray());
                    }
                }
            }
        }
    }
}
