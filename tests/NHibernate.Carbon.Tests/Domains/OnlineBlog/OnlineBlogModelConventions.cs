using NHibernate.Carbon.AutoPersistance.Core;
using NHibernate.Carbon.Tests.Domains.OnlineBlog.Model;

namespace NHibernate.Carbon.Tests.Domains.OnlineBlog
{
	public class OnlineBlogModelConventions
	{
		public AutoPersistanceModel GetPersistanceModel()
		{
			var model = new AutoPersistanceModel(GetConventions());

			// or use (sparingly as these conventions are not exspansive): 
			// AutoPersistanceModel model  = new AutoPeristanceModel(); //uses defaults for conventions (same as above)

			// set the assembly for the domain model to be auto-persisted and the hbm files will be constructed 
			// on each instance of accessing the NHibernate infrastructure to persist our model:
			model.Assembly(typeof (Blog).Assembly);
			model.Namespace.EndsWith("OnlineBlog.Model");
		
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
			
			convention.BaseEntitiesOnType(typeof(Entity<>));

			// can use custom column naming strategy as a convention:
			// convention.SetColumnNamingStrategy(GetColumnNamingStrategy());

			return convention;
		}
	}
}