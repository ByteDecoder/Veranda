using System;
using RedOwl.Core;
using UnityEngine;

namespace RedOwl.Veranda
{
    public static class VerandaUtils
    {
        public static GraphCache Graphs = new GraphCache();
        public static NodeCache Nodes = new NodeCache();
        public static PortCache Ports = new PortCache();
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            //Debug.Log("Building Veranda Reflection Caches");
            Graphs = new GraphCache();
            Nodes = new NodeCache();
            Ports = new PortCache();
            Graphs.ShouldBuildCache();
            Nodes.ShouldBuildCache();
            Ports.ShouldBuildCache();
        }

        internal static string GenerateId() => Guid.NewGuid().ToString();

        internal static Rect GenerateRect<T>(T _)
        {
            var attr = typeof(T).SafeGetAttribute<NodeSettingsAttribute>(true);
            return new Rect(0, 0, attr.Width, attr.Height);
        }
    }
}