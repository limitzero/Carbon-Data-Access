using NHibernate.Carbon.AutoPersistance.Core;

namespace NHibernate.Carbon.AutoPersistance
{
    /// <summary>
    /// This can be used as an example of the conventions that are enacted over the domain model:
    /// </summary>
    public class LocalMappingConventionProvider : IConventionProvider
    {
        #region IConventionProvider Members

        public ModelConvention GetConventions()
        {
            var convention = new ModelConvention();
            convention.PluralizeTableNames();
            convention.MemberAccess.NoSetterLowerCaseUnderscore();

            convention.PrimaryKey.PropertyName("Id"); //exclude primary keys in property definition.
            convention.PrimaryKey.RenderAsLowerCaseEntityNameFollowedByID();
            convention.PrimaryKey.MemberAccess.NoSetterLowerCaseUnderscore();

            convention.ForeignKey.RenderAsParentEntityHasInstancesOfChildEntity();
            convention.ManyToManyTableName.RenderAsParentEntityNameConcatenatedWithChildEntityNamePluralized();

            convention.Property.SetLargeTextFieldLengthsAndNames(100000, "Comments", "Notes", "Body", "History", "Message", "Event", "Text", "Content");
            convention.Property.SetDefaultTextFieldLength(200);
            convention.Property.RenderAsLowerCaseInRepository();

            return convention;
        }

        #endregion
    }
}