using System.IO;
using ImageMagick;
using Microsoft.AspNetCore.Hosting;

namespace WeebReader.Web.Services
{
    internal static class Utilities
    {
        public static DirectoryInfo GetContentFolder(IWebHostEnvironment environment) => new DirectoryInfo($"{environment.WebRootPath}{Path.DirectorySeparatorChar}content");
        
        public static MagickImage ProcessImage(Stream image)
        {
            var magickImage = new MagickImage(image);

            if (magickImage.Format != MagickFormat.Gif)
            {
                magickImage.Alpha(AlphaOption.Remove);
                magickImage.BackgroundColor = MagickColor.FromRgb(255, 255, 255);
                
                if (magickImage.DetermineColorType() == ColorType.Grayscale)
                    magickImage.Grayscale();
            }

            return magickImage;
        }

        public static FileInfo WriteImage(DirectoryInfo folder, MagickImage image, string filename, bool forceStatic = false)
        {
            string extension;
            MagickFormat format;

            if (forceStatic)
            {
                extension = ".png";
                format = image.ColorType == ColorType.Grayscale ? MagickFormat.Png8 : MagickFormat.Png24;
            }
            else
            {
                if (image.Format == MagickFormat.Gif)
                {
                    extension = ".gif";
                    format = image.Format;
                }
                else
                {
                    extension = ".png";
                    format = image.ColorType == ColorType.Grayscale ? MagickFormat.Png8 : MagickFormat.Png24;
                }
            }

            var file = new FileInfo($"{folder}{Path.DirectorySeparatorChar}{filename}{extension}");
            image.Write(file, format);

            return file;
        }
    }
}