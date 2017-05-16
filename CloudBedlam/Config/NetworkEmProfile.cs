using System;
using System.Linq;
using System.Reflection;

namespace CloudBedlam.Config
{
    public enum NetworkEmProfile
    {
       [NetEmProfile("bandwidth")]
       Bandwidth,
       [NetEmProfile("latency")]
       Latency,
       [NetEmProfile("disconnect")]
       Disconnect,
       [NetEmProfile("loss")]
       Loss,
       Unknown
    }

    public static class NetworkProfileExtensions
    {
        //http://stackoverflow.com/a/1799401/294804
        public static string GetNetEmName(this NetworkEmProfile profile)
        {
            return ((NetEmProfileAttribute)
                typeof(NetworkEmProfile)
                    .GetTypeInfo()
                    .GetMember(profile.ToString()).First()
                    .GetCustomAttributes(typeof(NetEmProfileAttribute), false).First()
                ).Name;
        }
    }
}
