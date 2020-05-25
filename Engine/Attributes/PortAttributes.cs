using System;

namespace RedOwl.Sleipnir.Engine
{
    [AttributeUsage(AttributeTargets.Field)]
    public class DataInAttribute : Attribute
    {
        
    }
    
    [AttributeUsage(AttributeTargets.Field)]
    public class DataOutAttribute : Attribute
    {
        
    }
    
    [AttributeUsage(AttributeTargets.Field)]
    public class DataInOutAttribute : Attribute
    {
        
    }
    
    [AttributeUsage(AttributeTargets.Field)]
    public class FlowInAttribute : Attribute
    {
        public readonly string Name;

        public FlowInAttribute(string name)
        {
            Name = name;
        }
    }
    
    [AttributeUsage(AttributeTargets.Field)]
    public class FlowOutAttribute : Attribute
    {
        public readonly string Name;

        public FlowOutAttribute(string name)
        {
            Name = name;
        }
    }
}