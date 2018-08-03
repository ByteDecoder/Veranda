#if UNITY_EDITOR
namespace Sleipnir
{
    public interface IConnection
    {
        IKnob OutputKnob { get; }
        IKnob InputKnob { get; }

        bool DoesHaveValue { get; }
        float ValueWindowWidth { get; }
        bool IsExpanded { get; set; }
        object Value { get; set; }
    }
}
#endif