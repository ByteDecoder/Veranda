namespace RedOwl.Sleipnir.Engine
{
    public interface IPort
    {
        INode Node { get; }
        PortIO Io { get; }
        string Name { get; }
        string Id { get; }
    }
}