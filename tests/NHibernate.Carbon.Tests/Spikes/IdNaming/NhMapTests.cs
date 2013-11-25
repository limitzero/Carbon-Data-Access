using NHibernate.Carbon.AutoPersistance.Core;
using NHibernate.Carbon.AutoPersistance.Schema.Elements;
using NHibernate.Carbon.Tests.Domains.OnlineCart;
using NHibernate.Carbon.Tests.Domains.OnlineCart.Model;
using Xunit;

namespace NHibernate.Carbon.Tests.Spikes.IdNaming
{

	public class NHMapTests
	{
		[Fact]
		public void can_serialize_nhmap_for_entity()
		{
			var map = new NHMap().Build(new OnlineCartModelConventions().GetConventions(), typeof (Customer));
			var results = map.Serialize(); 

			System.Diagnostics.Debug.WriteLine(ORMUtils.FormatXMLDocument(results));
		}
	}

	//public class PostIDGeneratorConvention : IIDGenerationConventionInterceptor<Blog>
	//{
	//    public IdGenerationTypes IDGenerationType { get; private set; }
	
	//    public void Configure()
	//    {
	//        this.IDGenerationType = IdGenerationTypes.Identity;
	//    }
	//}
	
}