using System;

namespace CloudBedlam.Config
{
    [Serializable]
    public enum NetworkEmProfile
    {
		[NetEmProfile("bandwidth")]
		Bandwidth, 
		[NetEmProfile("corruption")]
		Corruption,
		[NetEmProfile("disconnect")]
		Disconnect,
		[NetEmProfile("latency")]
		Latency,
		[NetEmProfile("loss")]
		Loss,
		[NetEmProfile("reorder")]
		Reorder,
		Unknown
    }

    public static class NetworkProfileExtensions
    {
        //http://stackoverflow.com/a/1799401/294804
        public static string GetNetEmName(this NetworkEmProfile profile)
        {
            return ((NetEmProfileAttribute)
                typeof(NetworkEmProfile)
                    .GetMember(profile.ToString())[0]
                    .GetCustomAttributes(typeof(NetEmProfileAttribute), false)[0]
                ).Name;
        }
    }
}
