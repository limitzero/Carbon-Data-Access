using System;
using System.Collections.Generic;

namespace NHibernate.Carbon.Tests.Domains.OnlineCart.Model
{
	public class Customer
	{
		private int _id;
		public virtual int Id
		{
			get { return _id; }
			private set { _id = value; }
		}

		private Name _name;
		public virtual Name Name
		{
			get { return _name; }
			private set { _name = value; }
		}

		private BillingAddress _billingaddress;
		public virtual BillingAddress BillingAddress
		{
			get { return _billingaddress; }
			private set { _billingaddress = value; }
		}

		private ShippingAddress _shippingaddress;
		public virtual ShippingAddress ShippingAddress
		{
			get { return _shippingaddress; }
			private set { _shippingaddress = value; }
		}

		public virtual void ChangeName(Name name)
		{
			this.Name = name;
		}

		public virtual void ChangeBillingAddress(BillingAddress billingAddress)
		{
			this.BillingAddress = billingAddress;
		}

		public virtual void ChangeShippingAddress(ShippingAddress shippingAddress)
		{
			this.ShippingAddress = shippingAddress;
		}

		public virtual void UseBillingAddressForShipping()
		{
			ShippingAddress shippingAddress = new ShippingAddress(this.BillingAddress.Address1,
				this.BillingAddress.Address2,
				this.BillingAddress.City,
				this.BillingAddress.State,
				this.BillingAddress.PostalCode);
			this.ShippingAddress = shippingAddress;
		}

	}
}
