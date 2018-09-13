namespace Sleipnir
{
    public struct Connection
    {
        public Knob Output;
        public Knob Input;

        public Connection(Knob output, Knob input)
        {
            Output = output;
            Input = input;
        }
    }
}