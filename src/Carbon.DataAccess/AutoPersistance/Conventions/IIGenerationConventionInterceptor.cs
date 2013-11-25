using System;
using System.Linq.Expressions;

namespace NHibernate.Carbon.AutoPersistance.Conventions
{
	public interface IIDGenerationConventionInterceptor
	{
		IdGenerationTypes IDGenerationType { get; }
		void Configure();
	}

	public interface IIDGenerationConventionInterceptor<TEntity> :
		IIDGenerationConventionInterceptor where TEntity : class
	{
	}
}