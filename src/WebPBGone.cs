using System;
using System.IO;

using ImageMagick;

namespace WebPBGone
{
    internal class WebPBGoneHandler
    {
        static void Main(string[] args)
        {
            var pathToImage = args[0];

            if (args.Length == 1)
            {
                convertToPng(pathToImage);
            }
            else if (args[1] == "png")
            {
                convertToPng(pathToImage);
            }
            else if (args[1] == "jpg")
            {
                convertToJpg(pathToImage);
            }
        }

        /*
        Takes file path as a string, reads the file in, converts it to a png using the ImageMagick library, then create the new image in a .png format
        Writes it to the same filename, just with .png instead of .webm
        */
        static void convertToPng(string filePath)
        {
            var outputFile = Path.ChangeExtension(filePath, "png");
            Console.WriteLine(outputFile);
            using (var inputStream = new FileStream(filePath, FileMode.Open))
            {
                using (var image = new MagickImage(inputStream))
                {
                    using (var outputStream = new FileStream(outputFile, FileMode.Create))
                    {
                        image.Format = MagickFormat.Png;
                        image.Write(outputStream);
                    }
                }
            }
        }

        /* 
        This might be useless, I'm going to have it anyway for flexibility in case I decide to add a jpg option later
        */
        static void convertToJpg(string filePath)
        {
            var outputFile = Path.ChangeExtension(filePath, "jpg");
            using (var inputStream = new FileStream(filePath, FileMode.Open))
            {
                using (var image = new MagickImage(inputStream))
                {
                    using (var outputStream = new FileStream(outputFile, FileMode.Create))
                    {
                        image.Format = MagickFormat.Png;
                        image.Quality = 100;
                        image.Write(outputStream);
                    }
                }
            }
        }
    }
}
