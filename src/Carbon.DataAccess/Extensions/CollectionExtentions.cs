using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NHibernate.Carbon.AutoPersistance.Core;

namespace NHibernate.Carbon.Extensions
{
	public static class CollectionExtentions
	{
		public static string SerializeAll<T>(this IEnumerable<T> list) where T : class
		{
			return SerializeItemCollection(list); ;
		}

		public static string SerializeAll<T>(this IList<T> list) where T : class
		{
			return SerializeItemCollection(list);
		}

		public static string SerializeAll<T>(this List<T> list) where T : class
		{
			return SerializeItemCollection(list);
		}

		private static string SerializeItemCollection<T>(IEnumerable<T> collection) where T :class 
		{
			string results = string.Empty;

			foreach (var item in collection)
			{
				try
				{
					results += string.Concat(ORMUtils.Serialize(item), Environment.NewLine);
				}
				catch
				{
				}
			}

			return results;
		}
	}
}