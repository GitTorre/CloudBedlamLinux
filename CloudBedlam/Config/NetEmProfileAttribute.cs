using System;

namespace CloudBedlam.Config
{
    public class NetEmProfileAttribute : Attribute
    {
        public string Name { get; set; }
        public NetEmProfileAttribute(string name)
        {
            Name = name;
        }
    }
}
