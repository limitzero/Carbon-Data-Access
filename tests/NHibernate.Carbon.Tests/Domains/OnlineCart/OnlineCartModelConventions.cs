using System;
using NHibernate.Carbon.AutoPersistance.Core;
using NHibernate.Carbon.AutoPersistance.Strategies;
using NHibernate.Carbon.Tests.Domains.OnlineCart.Model;

namespace NHibernate.Carbon.Tests.Domains.OnlineCart
{
	public class OnlineCartModelConventions
	{
		public AutoPersistanceModel GetPersistanceModel()
		{
			var model = new AutoPersistanceModel(GetConventions());

			// or use (sparingly as these conventions are not exspansive): 
			// AutoPersistanceModel model  = new AutoPeristanceModel(); //uses defaults for conventions (same as above)

			// set the assembly for the domain model to be auto-persisted and the hbm files will be constructed 
			// on each instance of accessing the NHibernate infrastructure to persist our model:
			model.Assembly(typeof(Customer).Assembly);
			model.Namespace.EndsWith("OnlineCart.Model");
			//model.WriteMappingsTo(...) -> writes the *hbm.xml files to a location that you choose.

			return model;
		}

		public ModelConvention GetConventions()
		{
			var convention = new ModelConvention();
			convention.PluralizeTableNames();
			convention.MemberAccess.NoSetterLowerCaseUnderscore();

			convention.Versioning.PropertyNameAndUnsavedValue("Version", 0);

			convention.PrimaryKey.PropertyName("Id"); //exclude primary keys in property definition.
			convention.PrimaryKey.RenderAsLowerCaseEntityNameFollowedByID();
			convention.PrimaryKey.MemberAccess.NoSetterCamelCaseUnderscore();

			convention.ForeignKey.RenderAsParentEntityHasInstancesOfChildEntity();
			convention.ManyToManyTableName.RenderAsParentEntityNameConcatenatedWithChildEntityNamePluralized();
		
			convention.Property.SetLargeTextFieldLengthsAndNames(10000, "Comments", "Notes", "Body", "History", "Message",
			                                                     "Event", "Text");
			convention.Property.SetDefaultTextFieldLength(200);
	
			// can use custom column naming strategy as a convention:
			convention.SetColumnNamingStrategy(GetColumnNamingStrategy());

			return convention;
		}

		private static IColumnNamingStrategy GetColumnNamingStrategy()
		{
			return new SampleColumnNamingStrategy();
		}

		public class SampleColumnNamingStrategy : IColumnNamingStrategy
		{
			public string Execute(string propertyName, System.Type propertyType)
			{
				string columnName = string.Empty;

				if (typeof(DateTime) == propertyType)
				{
					columnName = string.Concat("d", propertyName);
				}

				if (typeof(string) == propertyType)
				{
					columnName = string.Concat("c", propertyName);
				}

				if (typeof(decimal) == propertyType
					|| typeof(Decimal) == propertyType
					|| typeof(short) == propertyType
					|| typeof(int) == propertyType
					|| typeof(Int16) == propertyType
					|| typeof(Int32) == propertyType
					|| typeof(Int64) == propertyType
					|| typeof(Single) == propertyType
					|| typeof(float) == propertyType)
				{
					columnName = string.Concat("n", propertyName);
				}

				return columnName;
			}
		}
	}
}