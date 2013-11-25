using System;
using NHibernate.Carbon.AutoPersistance.Core;
using NHibernate.Carbon.AutoPersistance.Strategies;

namespace NHibernate.Carbon.Tests
{
	public class TestConfigurator
	{
		public static AutoPersistanceModel GetModel()
		{
			var model = new AutoPersistanceModel(GetConventions());

			// or use (sparingly as these conventions are not exspansive): 
			// AutoPersistanceModel model  = new AutoPeristanceModel(); //uses defaults for conventions (same as above)

			//set the assembly for the domain model to be auto-persisted:
			model.Assembly(typeof(TestConfigurator).Assembly);
			model.ConfigurationFile(@"hibernate.cfg.xml");
			// model.WriteMappingsTo(@"C:\repositories\Carbon-Data-Access\tests\Carbon.DataAccess.Tests\Domain");

			return model;
		}

		private static ModelConvention GetConventions()
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
			// renders collections as 'bag' by default => convention.ManyToManyTableName.RenderAsSet(); 

			convention.Property.SetLargeTextFieldLengthsAndNames(10000, "Comments", "Notes", "Body", "History", "Message",
			                                                     "Event", "Text");
			convention.Property.SetDefaultTextFieldLength(200);
			convention.Property.RenderAsLowerCaseInRepository();

			// can use custom column naming strategy as a convention:
			//convention.SetColumnNamingStrategy(GetColumnNamingStrategy());

			return convention;
		}

		private static IColumnNamingStrategy GetColumnNamingStrategy()
		{
			return new SampleColumnNamingStrategy();
		}

		// sample class to map the column names for the domain objects:

		#region Nested type: SampleColumnNamingStrategy

		public class SampleColumnNamingStrategy : IColumnNamingStrategy
		{
			public string Execute(string propertyName, System.Type propertyType)
			{
				string columnName = string.Empty;

				if (typeof (DateTime) == propertyType)
				{
					columnName = string.Concat("d", propertyName);
				}

				if (typeof (string) == propertyType)
				{
					columnName = string.Concat("c", propertyName);
				}

				if (typeof (decimal) == propertyType
				    || typeof (Decimal) == propertyType
				    || typeof (short) == propertyType
				    || typeof (int) == propertyType
				    || typeof (Int16) == propertyType
				    || typeof (Int32) == propertyType
				    || typeof (Int64) == propertyType
				    || typeof (Single) == propertyType
				    || typeof (float) == propertyType)
				{
					columnName = string.Concat("n", propertyName);
				}

				return columnName;
			}
		}

		#endregion
	}
}