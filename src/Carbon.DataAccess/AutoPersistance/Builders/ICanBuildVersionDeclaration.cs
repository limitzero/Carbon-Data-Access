using System.Reflection;

namespace NHibernate.Carbon.AutoPersistance.Builders
{
	public interface ICanBuildVersionDeclaration : IBuilder
	{
		PropertyInfo Property { get; set; }
	}
}