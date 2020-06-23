using System;
using System.IO;

namespace WeebReader.Common.Utilities
{
    public static class Location
    {
        public static DirectoryInfo CurrentDirectory => new DirectoryInfo(Environment.CurrentDirectory);
    }
}