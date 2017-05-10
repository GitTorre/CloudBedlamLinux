using System.IO;

namespace CloudBedlam.Extensions
{
    public static class DirectoryInfoExtensions
    {
        public static DirectoryInfo CreateEx(this DirectoryInfo directory)
        {
            directory.Create();
            return directory;
        }
    }
}
