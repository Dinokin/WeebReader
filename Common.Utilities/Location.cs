using System;
using System.IO;
using System.Reflection;

namespace WeebReader.Common.Utilities
{
    public static class Location
    {
        public static DirectoryInfo CurrentDirectory => new FileInfo(Uri.UnescapeDataString(new Uri(Assembly.GetExecutingAssembly().CodeBase!).AbsolutePath)).Directory;
    }
}