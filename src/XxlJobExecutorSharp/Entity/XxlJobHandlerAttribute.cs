using System;

namespace XxlJobExecutorSharp.Entity
{
    [AttributeUsage(AttributeTargets.Class)]
    public class XxlJobHandlerAttribute : Attribute
    {
        public string Name { get; }

        public Type InterfaceType { get; set; }

        public XxlJobHandlerAttribute(string name, Type interfaceType)
        {
            Name = name;
            InterfaceType = interfaceType;
        }

        public XxlJobHandlerAttribute(string name)
        {
            Name = name;
        }
    }
}
