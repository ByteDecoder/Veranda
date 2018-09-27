namespace Sleipnir.Mapper
{
    public class SubSlot
    {
        public string FieldName;
        public SlotAttribute Attribute;
        
        public SubSlot(SlotAttribute attribute, string fieldName)
        {
            Attribute = attribute;
            FieldName = fieldName;
        }
    }
}