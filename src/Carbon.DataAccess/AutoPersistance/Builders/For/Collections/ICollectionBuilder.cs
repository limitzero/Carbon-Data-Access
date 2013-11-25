using System.Reflection;
using NHibernate.Carbon.AutoPersistance.Core;

namespace NHibernate.Carbon.AutoPersistance.Builders.For.Collections
{
	/// <summary>
	/// Contract for building collection definitions between persistent entities.
	/// </summary>
	public interface ICollectionBuilder
	{
		bool IsApplicable(ModelConvention modelConvention, PropertyInfo property);
		bool IsBiDirectional { get; set; }
		void Build(ModelConvention modelConvention, string parentEntityPropertyName, System.Type parentEntity, System.Type childEntity, PropertyInfo property);
		string Serialize();
	}
}