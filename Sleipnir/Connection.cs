namespace Sleipnir
{
    public struct Connection
    {
        public Knob OutputKnob { get; }
        public Knob InputKnob { get; }

        public Connection(Knob outputKnob, Knob inputKnob)
        {
            OutputKnob = outputKnob;
            InputKnob = inputKnob;
        }
    }
}