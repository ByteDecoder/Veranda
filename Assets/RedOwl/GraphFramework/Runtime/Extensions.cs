using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace RedOwl.GraphFramework
{
	public static class Extensions
	{
		public static IEnumerable<Type> ForAllTypes<T>()
		{
			var assemblies = AppDomain.CurrentDomain.GetAssemblies();
			return assemblies.SelectMany(assembly => assembly.GetTypes()).Where(t => typeof(T).IsAssignableFrom(t) && !t.IsAbstract);
		}
		
		public static void WithAttr<T>(this Type self, Action<T> callback, bool inherit = true) where T : Attribute
		{
			var attrs = self.GetCustomAttributes(inherit);
			foreach (var item in attrs)
			{
				T attr = item as T;
				if (attr != null) callback(attr);
			}
		}
		
		public static void WithAttr<T>(this MemberInfo self, Action<T> callback, bool inherit = true) where T : Attribute
		{
			var attrs = self.GetCustomAttributes(inherit);
			foreach (var item in attrs)
			{
				T attr = item as T;
				if (attr != null) callback(attr);
			}
		}
	}
}
