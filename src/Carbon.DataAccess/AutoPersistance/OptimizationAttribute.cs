using System;

namespace NHibernate.Carbon.AutoPersistance
{
	/// <summary>
	/// Attribute to denote the optimization hints used in mapping for auto-persistance.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class OptimizationAttribute : Attribute 
	{
		public string Description { get; private set; }

		public OptimizationAttribute(string description)
		{
			Description = description;
		}
	}
}