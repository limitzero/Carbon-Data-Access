using System;
using Carbon.Repository.AutoPersistance.Core;
using Carbon.Repository.AutoPersistance.Persistance.Strategies;
using Carbon.Repository.AutoPersistance.Strategies;

namespace Carbon.DataAccess.Tests
{
    public class Configurator
    {
        static Configurator()
        {

        }

        public static AutoPersistanceModel GetModel()
        {
            var model = new AutoPersistanceModel(new DefaultPersistanceStrategy(), GetConventions());

            // or use (sparingly as these conventions are not exspansive): 
            // AutoPersistanceModel model  = new AutoPeristanceModel(); //uses defaults for conventions (same as above)

            //set the assembly for the domain model to be auto-persisted:
            model.Assembly("Carbon.DataAccess.Tests");
            model.Namespace.EndsWith("Domain");

            model.WriteMappingsTo(@"C:\repositories\Carbon-Data-Access\tests\Carbon.DataAccess.Tests\Domain");

            return model;
        }

        private static Convention GetConventions()
        {
            Convention convention = new Convention();
            convention.PluralizeTableNames();
            convention.MemberAccess.NoSetterLowerCaseUnderscore();

            convention.PrimaryKey.PropertyName("Id"); //exclude primary keys in property definition.
            convention.PrimaryKey.RenderAsLowerCaseEntityNameFollowedByID();
            convention.PrimaryKey.MemberAccess.NoSetterCamelCaseUnderscore();

            convention.ForeignKey.RenderAsParentEntityHasInstancesOfChildEntity();
            convention.ManyToManyTableName.RenderAsParentEntityNameConcatenatedWithChildEntityNamePluralized();

            convention.Property.SetLargeTextFieldLengthsAndNames(10000, "Comments", "Notes", "Body", "History", "Message", "Event", "Text");
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
        public class SampleColumnNamingStrategy : IColumnNamingStrategy
        {
            public string Execute(string propertyName, Type propertyType)
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