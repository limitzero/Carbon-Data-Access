using System;
using NHibernate.Carbon.AutoPersistance.Strategies;

namespace NHibernate.Carbon.AutoPersistance.Core
{
	public class DefaultPropertyNamingStrategy : IColumnNamingStrategy
	{
		public string Execute(string propertyName, System.Type propertyType)
		{
			return propertyName.ToLower();
		}
	}
}