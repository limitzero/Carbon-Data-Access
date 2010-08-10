using Carbon.DataAccess.Tests.Domain;
using Carbon.Repository.ForTesting;
using Xunit;

namespace Carbon.DataAccess.Tests.AutoPersistance
{

    public class PersistanceWithBaseClassTests : BaseAutoPersistanceTestFixture
    {
        public PersistanceWithBaseClassTests()
        {
            var model = Configurator.GetModel();
            model.CacheMapping(); // maps already created..use the existing *.hbm.xml files.

            OneTimeInitalizeWithModel(Configurator.GetModel());
            CreateUnitOfWork();
        }

        [Fact]
        public void Can_save_product_with_persistance_model()
        {
            var product = new Product("Windex", "Window/Tile Cleaner", 24.95M);
            Repository.Save<Product>(product);

            var fromDB = Repository.FindById<Product>(product.Id);
            Assert.Equal(product.Id, fromDB.Id); // "The identifier for the product could not be found."
        }

        [Fact]
        public void Can_save_order_with_persistance_model()
        {
            var order = new Order(new OrderStatus(OrderStatusEnum.Created), new OrderPriority(OrderPriorityEnums.NextDayAir));
            Repository.Save<Order>(order);

            var fromDB = Repository.FindById<Order>(order.Id);
            Assert.Equal(order.Id, fromDB.Id); //  "The identifier for the order could not be found."
        }

        [Fact]
        public void Can_save_line_item_to_order_with_persistance_model()
        {
            var order = new Order(new OrderStatus(OrderStatusEnum.Created), new OrderPriority(OrderPriorityEnums.NextSecond));
            order.AddItem(new OrderLine(new Product("Windex", "Glass Cleaner", 8.95M), 4));

            Repository.Save<Order>(order);

            var fromDB = Repository.FindById<Order>(order.Id);
            Assert.Equal(order.Id, fromDB.Id); // "The identifier for the order could not be found."
            Assert.Equal(1, fromDB.Orderlines.Count); // "The items for the order could not be retrieved."
        }

        [Fact]
        public void Can_save_orderline_with_persistance_model()
        {
            var item = new OrderLine(new Product("Windex", "Glass Cleaner", 8.95M), 4);
            Repository.Save<OrderLine>(item);

            var fromDB = Repository.FindById<OrderLine>(item.Id);
            Assert.Equal(item.Id, fromDB.Id); // "The identifier for the order line could not be found.");            
        }

    }
}