using System;
using System.IO;

namespace CloudBedlam.Extensions
{
    public static class PathEx
    {
        public static string GetSystemDriveLetter()
        {
            return Path.GetPathRoot(Environment.SystemDirectory).Substring(0, 1);
        }
    }
}
