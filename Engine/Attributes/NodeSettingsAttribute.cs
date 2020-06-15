using System;

namespace RedOwl.Sleipnir
{
    [AttributeUsage(AttributeTargets.Class)]
    public class NodeSettingsAttribute : Attribute
    {
        public float Width;
        public float Height;

        public NodeSettingsAttribute()
        {
            Width = 200;
            Height = 70;
        }
    }
}