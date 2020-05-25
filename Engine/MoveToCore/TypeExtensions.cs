using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RedOwl.Core
{
    public static class TypeExtensions
    {
        public static IEnumerable<Type> SafeGetTypes(this Assembly self)
        {
            try
            {
                return self.GetTypes();
            }
            catch
            {
                return Type.EmptyTypes;
            }
        }
        
        public static string SafeGetName(this Type self)
        {
            string output = "";
            var type = self;
            while (true)
            {
                output = type.FullName;
                if (string.IsNullOrEmpty(output))
                {
                    output = self.GetGenericTypeDefinition().FullName;
                    if (!string.IsNullOrEmpty(output))
                    {
                        return output;
                    }
                }
                else
                {
                    return output;
                }

                type = type.BaseType;
            }
        }

        public static TAttr SafeGetAttribute<TAttr>(this Type self, bool inherit = false) where TAttr : Attribute, new()
        {
            var attrType = typeof(TAttr);
            return self.IsDefined(attrType, inherit) ? (TAttr)self.GetCustomAttributes(attrType, inherit)[0] : new TAttr();
        }

        public static bool TryGetAttribute<TAttr>(this FieldInfo self, out TAttr attr, bool inherit = false) where TAttr : Attribute
        {
            var attrType = typeof(TAttr);
            attr = null;
            bool found = self.IsDefined(attrType, inherit);
            if (found)
            {
                attr = (TAttr) self.GetCustomAttributes(attrType, inherit)[0];
            }
            return found;
        }

        public static IEnumerable<Type> GetAllTypes<T>()
        {
            var attrType = typeof(T);
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.GlobalAssemblyCache) continue;
                foreach (var type in assembly.SafeGetTypes())
                {
                    if (attrType.IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface)
                        yield return type;
                }
            }
        }
        
        public static IEnumerable<Type> GetAllTypesWithAttribute<T>() where T : Attribute
        {
            var attrType = typeof(T);
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.GlobalAssemblyCache) continue;
                foreach (var type in assembly.SafeGetTypes())
                {
                    if (!type.IsAbstract && !type.IsInterface && type.IsDefined(attrType, true))
                        yield return type;
                }
            }
        }
        
        public static IEnumerable<FieldInfo> GetAllFieldsWithAttribute<T>(this Type type) where T : Attribute
        {
            var attrType = typeof(T);
            foreach (var info in type.GetFields().Where(p => p.IsDefined(attrType, false)))
            {
                yield return info;
            }
        }
    }
}