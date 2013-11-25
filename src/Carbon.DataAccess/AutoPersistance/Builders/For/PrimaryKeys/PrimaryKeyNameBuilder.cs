using NHibernate.Carbon.AutoPersistance.Core;

namespace NHibernate.Carbon.AutoPersistance.Builders.For.PrimaryKeys
{
    public class PrimaryKeyNameBuilder : IPrimaryKeyNameBuilder
    {
        private ModelConvention _convention;
        private System.Type _entity;

		public ModelConvention Convention
		{
			get
			{
				return _convention;
			}
			set
			{
				_convention = value;
			}
		}

		public System.Type Entity
		{
			get
			{
				return _entity;
			}
			set
			{
				_entity = value;
			}
		}

        public PrimaryKeyNameBuilder()
            : this(null, null)
        {

        }

        public PrimaryKeyNameBuilder(ModelConvention convention, System.Type entity)
        {
            _convention = convention;
            _entity = entity;
        }

        public string Build()
        {
            string retval = string.Empty;

            if (_convention.PrimaryKey.IsEntityNameFollowedByID)
                retval = string.Concat(_entity.Name, "ID");

            if (_convention.PrimaryKey.IsLowerCaseEntityNameFollowedByID)
                retval = string.Concat(_entity.Name.ToLower(), "ID");

            if (_convention.PrimaryKey.IsLowerCasePKUnderscoreEntityName)
                retval = string.Concat("pk_", _entity.Name);

            if (_convention.PrimaryKey.IsLowerCasePKUnderscoreEntityNameUnderscoreID)
                retval = string.Concat("pk_", _entity.Name, "_ID");

            return retval;
        }

    }
}