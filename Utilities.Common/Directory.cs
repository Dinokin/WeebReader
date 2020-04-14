using System;
using System.IO;
using System.Reflection;

namespace WeebReader.Utilities.Common
{
    public static class Directory
    {
        public static DirectoryInfo CurrentDirectory => new FileInfo(Uri.UnescapeDataString(new Uri(Assembly.GetExecutingAssembly().CodeBase!).AbsolutePath)).Directory;
    }
}