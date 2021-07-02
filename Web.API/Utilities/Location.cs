using System.IO;
using System.Reflection;

namespace WeebReader.Web.API.Utilities
{
    public static class Location
    {
        public static DirectoryInfo CurrentDirectory => new FileInfo(Assembly.GetExecutingAssembly().Location).Directory!;
    }
}