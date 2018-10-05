using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.Utilities;

namespace Sleipnir.Mapper
{
    [Serializable]
    public class Nest
    {
        public string FieldName;
        public bool Drawn = false;
        public float YPosition = 0;
        public List<Nest> Nests;
        public List<Tuple<SlotAttribute, string>> Slots;

        public Nest(object toMap, string fieldName)
        {
            Nests = new List<Nest>();
            FieldName = fieldName;

            Slots = toMap
                .GetType()
                .GetFields()
                .Where(p => p.IsDefined(typeof(SlotAttribute), true))
                .SelectMany(s =>
                {
                    var result = new List<Tuple<SlotAttribute, string>>();
                    var attribute = (SlotAttribute) s.GetCustomAttribute(typeof(SlotAttribute));
                    result.Add(new Tuple<SlotAttribute, string>(attribute, s.Name));
                    return result;
                })
                .ToList();
            
            var list = toMap as IList;
            if (list != null)
                for (var i = 0; i < list.Count; i++)
                    Nests.Add(new Nest(list[i], "[" + i + "]"));

            var nestedFields = toMap.GetType()
                .GetFields().Where(p => p.IsDefined(typeof(NestedAttribute), true))
                .ToArray();
            
            foreach (var nestedField in nestedFields)
            {
                var value = nestedField.GetValue(toMap);
                if (value == null)
                    continue;
                Nests.Add(new Nest(value, nestedField.Name));
            }
        }

        public List<Tuple<OdinSlot, SlotAttribute>> GetSlots<T>(int nodeIndex, string parentPath)
        {
            var path = parentPath.IsNullOrWhitespace()
                ? FieldName
                : parentPath + "." + FieldName;
            
            var subSlots = Slots
                .Select(subSlot => new Tuple<OdinSlot, SlotAttribute>
                (
                    new OdinSlot
                    { 
                        DeepReflectionPath = path.IsNullOrWhitespace()
                            ? subSlot.Item2
                            : path + "." + subSlot.Item2,
                        NodeIndex = nodeIndex
                    }, 
                    subSlot.Item1
                ))
                .ToList();

            return subSlots.Concat(Nests.SelectMany(nest => nest.GetSlots<T>(nodeIndex, path))).ToList();
        }
    }
}