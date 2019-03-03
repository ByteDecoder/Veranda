using System;

namespace RedOwl.GraphFramework
{
    [Flags]
    public enum PortDirections
    {
        None = 0,
        Input = 1 << 0,
        Output = 1 << 1,

        InOut = Input | Output,
    }

    public static class PortDirectionExtensions
    {
        public static bool IsInput(this PortDirections self)
        {
            return self.HasFlag(PortDirections.Input);
        }

        public static bool IsOutput(this PortDirections self)
        {
            return self.HasFlag(PortDirections.Output);
        }
    }
}
