using System.IO;
using System.Reflection;

namespace WeebReader.Web.API.Utilities
{
    public static class Location
    {
        public static readonly DirectoryInfo CurrentDirectory = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory!;
        public static readonly DirectoryInfo StaticDirectory = Directory.CreateDirectory($"{Location.CurrentDirectory}{Path.DirectorySeparatorChar}Static");
    }
}