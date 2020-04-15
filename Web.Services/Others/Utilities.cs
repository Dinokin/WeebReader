using System.IO;
using ImageMagick;
using Microsoft.AspNetCore.Hosting;

namespace WeebReader.Web.Services.Others
{
    internal static class Utilities
    {
        internal static DirectoryInfo GetContentFolder(IWebHostEnvironment environment) => new DirectoryInfo($"{environment.WebRootPath}{Path.DirectorySeparatorChar}content");
        
        internal static MagickImage ProcessImage(Stream image, bool disposeStream = true)
        {
            var magickImage = new MagickImage(image);
            
            magickImage.Alpha(AlphaOption.Remove);
            magickImage.BackgroundColor = MagickColor.FromRgb(255, 255, 255);
                
            if (magickImage.DetermineColorType() == ColorType.Grayscale)
                magickImage.Grayscale();

            if (disposeStream)
                image.Dispose();
            
            return magickImage;
        }

        internal static MagickImageCollection ProcessAnimation(Stream image, bool disposeStream = true)
        {
            var imageCollection = new MagickImageCollection(image);

            if (disposeStream)
                image.Dispose();

            return imageCollection;
        }

        internal static FileInfo WriteImage(DirectoryInfo folder, MagickImage image, string filename, bool useJpg = false, bool disposeImage = true)
        {
            var file = new FileInfo($"{folder}{Path.DirectorySeparatorChar}{filename}.png");
            var format = useJpg ? image.Format = MagickFormat.Jpg : image.ColorType == ColorType.Grayscale ? MagickFormat.Png8 : MagickFormat.Png24;

            image.Write(file, format);

            if (disposeImage) 
                image.Dispose();

            return file;
        }

        internal static FileInfo WriteAnimation(DirectoryInfo folder, MagickImageCollection image, string filename, bool disposeImage = true)
        {
            var file = new FileInfo($"{folder}{Path.DirectorySeparatorChar}{filename}.gif");
            image.Write(file, MagickFormat.Gif);

            if (disposeImage) 
                image.Dispose();

            return file;
        }
    }
}