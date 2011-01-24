namespace Carbon.DataAccess.Tests.Domain
{
    /// <summary>
    /// ShippingAddress "is a" Address, but the entity that should 
    /// be preserved is the the ShippingAddress, not the base value type
    /// this is why it needs an identifier column.
    /// </summary>
    public class ShippingAddress : Address
    {
        private int _id;

        public virtual int Id
        {
            get
            {
                return _id;
            }
        }

        public ShippingAddress()
        { }

       
    }
}