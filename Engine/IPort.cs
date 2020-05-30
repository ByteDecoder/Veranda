namespace RedOwl.Sleipnir.Engine
{
    public interface IPort
    {
        string Id { get; }
        PortIO Io { get;  }
        INode Node { get; }
    }
}