using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.Utilities;

namespace Sleipnir.Mapper
{
    public class Nest
    {
        public string FieldName;
        public int Indexer;
        public bool Drawn = false;
        public bool DisplayWhenHidden;
        public float YPosition = 0;
        public List<SubSlot> Slots;
        public List<Nest> Nests;

        public Nest(object toMap, string fieldName, int indexer = -1, bool displayWhenHidden = true)
        {
            Nests = new List<Nest>();
            Indexer = indexer;
            FieldName = fieldName;
            DisplayWhenHidden = displayWhenHidden;

            Slots = toMap
                .GetType()
                .GetFields()
                .Where(p => p.IsDefined(typeof(SlotAttribute), true))
                .SelectMany(s =>
                {
                    var result = new List<SubSlot>();
                    var attribute = (SlotAttribute) s.GetCustomAttribute(typeof(SlotAttribute));
                    if(attribute.Direction.IsInput())
                        result.Add(new SubSlot(SlotDirection.Input, s.Name));
                    if (attribute.Direction.IsOutput())
                        result.Add(new SubSlot(SlotDirection.Output, s.Name));
                    return result;
                })
                .ToList();
            
            var nestedFields = toMap.GetType()
                .GetFields().Where(p => p.IsDefined(typeof(NestedAttribute), true))
                .ToArray();
            
            foreach (var nestedField in nestedFields)
            {
                var value = nestedField.GetValue(toMap);
                if (value == null)
                    continue;

                var attribute = (NestedAttribute)nestedField.GetCustomAttribute(typeof(NestedAttribute));
                var list = value as IList;
                if (list != null)
                    for (var i = 0; i < list.Count; i++)
                        Nests.Add(new Nest(list[i], nestedField.Name, i, displayWhenHidden));
                Nests.Add(new Nest(value, nestedField.Name, displayWhenHidden: attribute.DisplayWhenHidden));
            }
        }

        public List<OdinSlot> GetSlots<T>(int nodeIndex, string parentPath)
        {
            var path = parentPath.IsNullOrWhitespace()
                ? FieldName
                : parentPath + "." + FieldName;

            if (Indexer != -1)
                path += ".[" + Indexer + "]";

            var subSlots = Slots
                .Select(subSlot =>
                    new OdinSlot{ 
                        DeepReflectionPath = path.IsNullOrWhitespace()
                            ? subSlot.FieldName
                            : path + "." + subSlot.FieldName,
                        NodeIndex = nodeIndex
                    })
                .ToList();

            var result = subSlots.Concat(Nests.SelectMany(nest => nest.GetSlots<T>(nodeIndex, path))).ToList();
            return result;
        }
    }
}