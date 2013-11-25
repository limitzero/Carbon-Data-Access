using NHibernate.Carbon.AutoPersistance.Core;

namespace NHibernate.Carbon.AutoPersistance.Builders.For.ForeignKeys
{
    public class ForeignKeyNameBuilder : IForeignKeyNameBuilder
    {
        private ModelConvention _convention;
        private System.Type _parent;
        private readonly System.Type _child;

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
				return _parent;
			}
			set
			{
				_parent = value;
			}
		}

        public ForeignKeyNameBuilder(ModelConvention convention, System.Type parent, System.Type child)
        {
            _convention = convention;
            _parent = parent;
            _child = child;
        }

        public string Build()
        {
            string results = string.Empty;

            if (_convention.ForeignKey.CanRenderAsParentEntityHasInstancesOfChildEntity)
            {
                results = string.Format("fk_{0}_has_instances_of_{1}", _parent.Name, _child.Name);
            }
            else if (_convention.ForeignKey.CanRenderAsParentEntityHasPluralizedChildEntities)
            {
                results = string.Format("fk_{0}_has_{1}", _parent.Name, ORMUtils.Pluralize(_child.Name));
            }

            return results;
        }
    }
}