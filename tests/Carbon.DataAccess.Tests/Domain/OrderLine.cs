using System;
using System.Collections.Generic;
using System.Text;

namespace Carbon.DataAccess.Tests.Domain
{
    public class OrderLine
    {
    	
    	private Product _product;
        private int _quantity = 0;
        private int _id = 0;

		public virtual int Id
		{
			get
			{
				return _id;
			}

		}

		/// <summary>
		/// Read-Only. Instance of the product that should be attached to the order line.
		/// </summary>
		public virtual Product Product
		{
			get
			{
				return _product;
			}
			set
			{
				_product = value;
			}
		}

		public virtual int Quantity
		{
			get
			{
				return _quantity;
			}
			set
			{
				_quantity = value;
			}
		}

    	private Order _order;
    	public virtual Order Order
    	{
    		get { return _order; }
    		private set { _order = value; }
    	}

    	protected OrderLine()
        { }

		public OrderLine(Order order)
		{
			Order = order;
		}

		public virtual Product CreateProduct()
		{
			return new Product(this);
		}

    	public virtual void SaveProduct(Product product, int quantity)
    	{
    		this.Product = product;
    		this.Quantity = quantity;
    	}

    }
}