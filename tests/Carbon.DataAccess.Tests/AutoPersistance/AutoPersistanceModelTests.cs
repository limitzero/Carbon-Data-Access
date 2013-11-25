using System.Diagnostics;
using NHibernate.Carbon.AutoPersistance.Core;
using Xunit;

namespace NHibernate.Carbon.Tests.AutoPersistance
{
	public class AutoPersistanceModelTests
	{
		private readonly AutoPersistanceModel m_model;

		public AutoPersistanceModelTests()
		{
			m_model = TestConfigurator.GetModel();
			m_model.ConfigurationFile(@"hibernate.cfg.xml");
		}

		[Fact]
		public void Can_scan_for_entities_for_model()
		{
			m_model.Build();
			Assert.Equal(8, m_model.Entities.Count);
				//"The HomeAddress sub-component is not a stand-alone entity since it is inherited from Address
		}

		[Fact]
		public void Can_set_model_to_generate_one_mapping_per_entity()
		{
			m_model.RenderMappingPerEntity();
			Assert.Equal(true, m_model.CanRenderMappingPerEntity);
			//"The setting to render a mapping per entity could not be retreived."
		}

		[Fact]
		public void Can_set_model_to_generate_one_mapping_for_all_entities()
		{
			m_model.Build();
			Debug.WriteLine(m_model.GetMaps());
			Assert.Equal(1, m_model.Maps.Count);
			//"The setting to render a mapping for all entities could not be retreived."
		}

		[Fact]
		public void Can_create_schema_based_on_model()
		{
			m_model.Build();
			m_model.CreateSchema();
			Assert.Equal(true, m_model.IsSchemaCreated); // "The schema could not be initialized."
		}

		[Fact]
		public void Can_drop_schema_based_on_model()
		{
			m_model.Build();
			m_model.DropSchema();
			Assert.Equal(true, m_model.IsSchemaDropped); //"The schema could not be dropped."
		}
	}
}