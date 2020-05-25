using System;
using UnityEngine;

namespace RedOwl.Sleipnir.Engine
{
    public interface IGraphOutput
    {
        DataPort Data { get; }
    }
    
    [Serializable]
    public abstract class GraphOutputNode<T> : Node, IGraphOutput
    {
        [DataIn]
        protected DataPort<T> data;

        protected GraphOutputNode()
        {
            data = new DataPort<T>();
        }

        public DataPort Data => data;
    }
    
    // System
    public class GraphOutputBoolean : GraphOutputNode<bool> {}
    public class GraphOutputSByte : GraphOutputNode<sbyte> {}
    public class GraphOutputByte : GraphOutputNode<byte> {}
    public class GraphOutputChar : GraphOutputNode<char> {}
    public class GraphOutputDouble : GraphOutputNode<double> {}
    public class GraphOutputDecimal : GraphOutputNode<decimal> {}
    public class GraphOutputInt : GraphOutputNode<int> {}
    public class GraphOutputUInt : GraphOutputNode<uint> {}
    public class GraphOutputLong : GraphOutputNode<long> {}
    public class GraphOutputULong : GraphOutputNode<ulong> {}
    public class GraphOutputShort : GraphOutputNode<short> {}
    public class GraphOutputUShort : GraphOutputNode<ushort> {}
    public class GraphOutputFloat : GraphOutputNode<float> {}
    public class GraphOutputString : GraphOutputNode<string> {}
    public class GraphOutputDataTime : GraphOutputNode<DateTime> {}
    public class GraphOutputDateTimeOffset : GraphOutputNode<DateTimeOffset> {}
    public class GraphOutputTimeSpan : GraphOutputNode<TimeSpan> {}

    // Unity
    public class GraphOutputUri : GraphOutputNode<Uri> {}
    public class GraphOutputGuid : GraphOutputNode<Guid> {}
    public class GraphOutputBounds : GraphOutputNode<Bounds> {}
    public class GraphOutputBoundsInt : GraphOutputNode<BoundsInt> {}
    public class GraphOutputGradient : GraphOutputNode<Gradient> {}
    public class GraphOutputColor : GraphOutputNode<Color> {}
    public class GraphOutputColor32 : GraphOutputNode<Color32> {}
    public class GraphOutputCurve : GraphOutputNode<AnimationCurve> {}
    public class GraphOutputLayerMask : GraphOutputNode<LayerMask> {}
    public class GraphOutputRect : GraphOutputNode<Rect> {}
    public class GraphOutputRectInt : GraphOutputNode<RectInt> {}
    public class GraphOutputTexture : GraphOutputNode<Texture> {}
    public class GraphOutputTexture2D : GraphOutputNode<Texture2D> {}
    public class GraphOutputRenderTexture : GraphOutputNode<RenderTexture> {}
    public class GraphOutputVector2 : GraphOutputNode<Vector2> {}
    public class GraphOutputVector2Int : GraphOutputNode<Vector2Int> {}
    public class GraphOutputVector3 : GraphOutputNode<Vector3> {}
    public class GraphOutputVector3Int : GraphOutputNode<Vector3Int> {}
    public class GraphOutputVector4 : GraphOutputNode<Vector4> {}
    public class GraphOutputQuaternion : GraphOutputNode<Quaternion> {}
}