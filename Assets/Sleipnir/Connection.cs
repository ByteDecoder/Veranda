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
}