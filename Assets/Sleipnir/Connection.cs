using UnityEngine;

namespace Sleipnir
{
    public struct Connection
    {
        public Slot Output;
        public Slot Input;

        public Connection(Slot output, Slot input)
        {
            Output = output;
            Input = input;
        }
    }
    
    public static class ConnectionExtensions
    {
        public static void Shlep(this Connection self)
        {
            Debug.LogFormat("'{0}[{1}]' => '{2}[{3}]'", self.Output.Node.Getter(), self.Output.PropertyPath, self.Input.Node.Getter(), self.Input.PropertyPath);
            var getter = self.Output.Getter();
            var setter = self.Input.Setter();
            setter(self.Input.Node.Getter(), getter(self.Output.Node.Getter()));
        }
    }
}