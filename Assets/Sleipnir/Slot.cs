using System;
using Sirenix.Utilities;

namespace Sleipnir
{
    public class Slot
    {
        public ValueWrappedNode Node;
        public string PropertyPath;

        public Slot(ValueWrappedNode node, string propertyPath)
        {
            Node = node;
            PropertyPath = propertyPath;
        }
    }
    
    public static class SlotExtensions 
    {
        public static Func<object, object> Getter(this Slot self)
        {
            return DeepReflection.CreateWeakInstanceValueGetter(self.Node.Getter().GetType(), typeof(Object), self.PropertyPath);
        }
        
        public static Action<object, object> Setter(this Slot self)
        {
            return DeepReflection.CreateWeakInstanceValueSetter(self.Node.Getter().GetType(), typeof(Object), self.PropertyPath);
        }
    }
}