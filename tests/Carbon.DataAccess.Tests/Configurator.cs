using Carbon.Repository.AutoPersistance.Core;
using Carbon.Repository.AutoPersistance.Persistance.Strategies;

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

            model.WriteMappingsTo(@"C:\Work\Playground\Carbon.DataAccess\tests\Carbon.DataAccess.Tests\Domain");

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

            return convention;
        }

    }
}