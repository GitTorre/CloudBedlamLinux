using System;
using System.Diagnostics;

namespace CloudBedlam.Extensions
{
    public static class ProcessExtensions
    {
        //http://stackoverflow.com/a/21482938/294804
        public static bool IsRunning(this Process process)
        {
            try { Process.GetProcessById(process.Id); }
            catch (InvalidOperationException) { return false; }
            catch (ArgumentException) { return false; }
            return true;
        }
    }
}
