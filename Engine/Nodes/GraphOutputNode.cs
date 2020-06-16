using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RedOwl.Veranda
{
    public interface IGraphOutput
    {
        DataPort Data { get; }
    }
    
    [Serializable]
    public abstract class GraphOutputNode<T> : Node, IGraphOutput
    {
        [SerializeField, HideLabel, HideReferenceObjectPicker, InlineProperty]
        protected DataIn<T> data;

        public DataPort Data => data;
    }
    
    // System
    [Serializable]
    public class GraphOutputBoolean : GraphOutputNode<bool> {}
    [Serializable]
    public class GraphOutputSByte : GraphOutputNode<sbyte> {}
    [Serializable]
    public class GraphOutputByte : GraphOutputNode<byte> {}
    [Serializable]
    public class GraphOutputChar : GraphOutputNode<char> {}
    [Serializable]
    public class GraphOutputDouble : GraphOutputNode<double> {}
    [Serializable]
    public class GraphOutputDecimal : GraphOutputNode<decimal> {}
    [Serializable]
    public class GraphOutputInt : GraphOutputNode<int> {}
    [Serializable]
    public class GraphOutputUInt : GraphOutputNode<uint> {}
    [Serializable]
    public class GraphOutputLong : GraphOutputNode<long> {}
    [Serializable]
    public class GraphOutputULong : GraphOutputNode<ulong> {}
    [Serializable]
    public class GraphOutputShort : GraphOutputNode<short> {}
    [Serializable]
    public class GraphOutputUShort : GraphOutputNode<ushort> {}
    [Serializable]
    public class GraphOutputFloat : GraphOutputNode<float> {}
    [Serializable]
    public class GraphOutputString : GraphOutputNode<string> {}
    [Serializable]
    public class GraphOutputDataTime : GraphOutputNode<DateTime> {}
    [Serializable]
    public class GraphOutputDateTimeOffset : GraphOutputNode<DateTimeOffset> {}
    [Serializable]
    public class GraphOutputTimeSpan : GraphOutputNode<TimeSpan> {}

    // Unity
    [Serializable]
    public class GraphOutputUri : GraphOutputNode<Uri> {}
    [Serializable]
    public class GraphOutputGuid : GraphOutputNode<Guid> {}
    [Serializable]
    public class GraphOutputBounds : GraphOutputNode<Bounds> {}
    [Serializable]
    public class GraphOutputBoundsInt : GraphOutputNode<BoundsInt> {}
    [Serializable]
    public class GraphOutputGradient : GraphOutputNode<Gradient> {}
    [Serializable]
    public class GraphOutputColor : GraphOutputNode<Color> {}
    [Serializable]
    public class GraphOutputColor32 : GraphOutputNode<Color32> {}
    [Serializable]
    public class GraphOutputCurve : GraphOutputNode<AnimationCurve> {}
    [Serializable]
    public class GraphOutputLayerMask : GraphOutputNode<LayerMask> {}
    [Serializable]
    public class GraphOutputRect : GraphOutputNode<Rect> {}
    [Serializable]
    public class GraphOutputRectInt : GraphOutputNode<RectInt> {}
    [Serializable]
    public class GraphOutputTexture : GraphOutputNode<Texture> {}
    [Serializable]
    public class GraphOutputTexture2D : GraphOutputNode<Texture2D> {}
    [Serializable]
    public class GraphOutputRenderTexture : GraphOutputNode<RenderTexture> {}
    [Serializable]
    public class GraphOutputVector2 : GraphOutputNode<Vector2> {}
    [Serializable]
    public class GraphOutputVector2Int : GraphOutputNode<Vector2Int> {}
    [Serializable]
    public class GraphOutputVector3 : GraphOutputNode<Vector3> {}
    [Serializable]
    public class GraphOutputVector3Int : GraphOutputNode<Vector3Int> {}
    [Serializable]
    public class GraphOutputVector4 : GraphOutputNode<Vector4> {}
    [Serializable]
    public class GraphOutputQuaternion : GraphOutputNode<Quaternion> {}
}