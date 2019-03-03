
namespace RedOwl.GraphFramework
{
	public abstract partial class Graph
	{
		public delegate void NodeAdded(Node node);
		public event NodeAdded OnNodeAdded;
		internal void FireNodeAdded(Node node)
		{
			OnNodeAdded?.Invoke(node);
			if (AutoExecute) Execute();
		}

		public delegate void NodeRemoved(Node node);
		public event NodeRemoved OnNodeRemoved;
		internal void FireNodeRemoved(Node node)
		{
			OnNodeRemoved?.Invoke(node);
			if (AutoExecute) Execute();
		}

		public delegate void ConnectionAdded(Connection connection);
		public event ConnectionAdded OnConnectionAdded;
		internal void FireConnectionAdded(Connection connection)
		{
			OnConnectionAdded?.Invoke(connection);
			if (AutoExecute) Execute();
		}

		public delegate void ConnectionRemoved(Connection connection);
		public event ConnectionRemoved OnConnectionRemoved;
		internal void FireConnectionRemoved(Connection connection)
		{
			OnConnectionRemoved?.Invoke(connection);
			if (AutoExecute) Execute();
		}
	}
}