namespace Sleipnir.Mapper
{
    public class SubSlot
    {
        public string FieldName;
        public SlotDirection Direction;

        public SubSlot(SlotDirection direction, string fieldName)
        {
            Direction = direction;
            FieldName = fieldName;
        }
    }
}