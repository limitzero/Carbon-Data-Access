using System.Text;
using NHibernate.Carbon.AutoPersistance.Core;
using NHibernate.Carbon.AutoPersistance.Schema.Elements;

namespace NHibernate.Carbon.AutoPersistance.Builders.For.PrimaryKeys
{
    public class PrimaryKeyBuilder : IPrimaryKeyBuilder
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

        public PrimaryKeyBuilder()
            : this(null, null)
        {

        }

        public PrimaryKeyBuilder(ModelConvention convention, System.Type entity)
        {
            _convention = convention;
            _entity = entity;
        }


        public string Build()
        {
            StringBuilder results = new StringBuilder();

        	var idColumn = _entity.GetProperty(_convention.PrimaryKey.PrimaryKeyName);

            if (idColumn != null)
            {
            	var property = new NHIDPropertyElement().Build(_convention, _entity);
            	results.Append(property.Serialize()).Append(System.Environment.NewLine);
            }

            return results.ToString();
        }

    }
}