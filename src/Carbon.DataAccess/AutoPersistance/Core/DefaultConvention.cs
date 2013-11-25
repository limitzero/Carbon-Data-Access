namespace NHibernate.Carbon.AutoPersistance.Core
{
	/// <summary>
	/// Default conventions used to set up entity configuration by NHibernate to the data store.
	/// </summary>
	public class DefaultConvention : ModelConvention
	{
		public DefaultConvention()
		{
			Initialize();
		}

		private void Initialize()
		{
			this.PluralizeTableNames();
			this.MemberAccess.NoSetterLowerCaseUnderscore();

			this.Versioning.PropertyNameAndUnsavedValue("Version", 0);

			//exclude primary keys in property definition and make this the basis for finding entitites.
			this.PrimaryKey.PropertyName("Id"); 

			this.PrimaryKey.RenderAsLowerCaseEntityNameFollowedByID();
			this.PrimaryKey.MemberAccess.NoSetterCamelCaseUnderscore();

			this.ForeignKey.RenderAsParentEntityHasInstancesOfChildEntity();
			this.ManyToManyTableName.RenderAsParentEntityNameConcatenatedWithChildEntityNamePluralized();

			this.Property.SetLargeTextFieldLengthsAndNames(10000, "Comments", "Notes", "Body", "History", "Message",
																 "Event", "Text");
			this.Property.SetDefaultTextFieldLength(200);
		}
	}
}