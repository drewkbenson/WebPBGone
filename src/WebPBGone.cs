using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImageMagick;

namespace WebPBGone
{
    internal class WebPBGoneHandler
    {
        //                                               0     1     2     3     4     5     6     7     8     9     10    11    12    13    14
        private static byte[] magicNumber = new byte[] { 0x52, 0x49, 0x46, 0x46, 0x00, 0x00, 0x00, 0x00, 0x57, 0x45, 0x42, 0x50, 0x56, 0x50, 0x38 };
        static void Main(string[] args)
        {
            var pathToImages = new List<String>();
            var deleting = false;
            var converting_to = "png";

            foreach (var item in args)
            {
                if (item.Equals("--delete"))
                {
                    deleting = true;
                }
                else if (item.Equals("--jpg"))
                {
                    converting_to = "jpg";
                }
                else
                {
                    if (CheckValidWebP(item))
                    {
                        pathToImages.Add(item);
                    }
                }
            }

            foreach (var pathToImage in pathToImages)
            {
                if (File.Exists(pathToImage))
                {
                    if (converting_to.Equals("png"))
                    {
                        convertToPng(pathToImage);
                    }
                    else
                    {
                        convertToJpg(pathToImage);
                    }

                    if (deleting)
                    {
                        File.Delete(pathToImage);
                    }
                }
            }

            // Old code - Ignore
            /*
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
            */
        }

        static bool CheckValidWebP(string pathToImage)
        {
            if (File.Exists(pathToImage))
            {
                if (!(File.GetAttributes(pathToImage).HasFlag(FileAttributes.Directory)))
                {
                    return checkMagicNumber(pathToImage);
                }
            }
            return false;
        }

        static bool checkMagicNumber(string pathToImage)
        {
            byte[] buffer = new byte[15];
            using (FileStream fs = new FileStream(pathToImage, FileMode.Open, FileAccess.Read))
            {
                var bytesRead = fs.Read(buffer, 0, buffer.Length);
                if (bytesRead < 15)
                {
                    return false;
                }
            }

            buffer[4] = 0;
            buffer[5] = 0;
            buffer[6] = 0;
            buffer[7] = 0;

            return buffer.SequenceEqual(magicNumber);
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
