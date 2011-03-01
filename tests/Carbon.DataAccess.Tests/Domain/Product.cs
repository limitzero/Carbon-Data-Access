using System;
using System.Collections.Generic;
using System.Text;

namespace Carbon.DataAccess.Tests.Domain
{
    // nullables put here for library support for auto-mapping:
    public class Product
    {
    	
    	private string _name = string.Empty;
        private string _description = string.Empty;
        private decimal? _price =0.0M;
        private int _id = 0;
        private DateTime? _createdon;

		public virtual int Id
		{
			get
			{
				return _id;
			}

		}

		public virtual DateTime? CreatedOn
		{
			get { return _createdon; }
			set { _createdon = value; }
		}

		public virtual string Description
		{
			get
			{
				return _description;
			}
			set
			{
				_description = value;
			}
		}

		public virtual string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}

		public virtual decimal? Price
		{
			get
			{
				return _price;
			}
			set
			{
				_price = value;
			}
		}

    	private OrderLine _orderline;
    	public virtual OrderLine OrderLine
    	{
    		get { return _orderline; }
    		private set { _orderline = value; }
    	}

    	protected Product()
        {

        }

    	public Product(OrderLine orderLine)
    	{
    		OrderLine = orderLine;
    	}


		public virtual void ChangeProduct(string name, string description, decimal price)
		{
			this.Name = name;
			this.Description = description;
			this.Price = price;
		}
    }
}