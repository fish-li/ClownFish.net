using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DEMO.Model
{
	public class TestSerializerModel
	{
		public string StringVal { get; set; }

		public int IntVal { get; set; }

		public DateTime DtValue { get; set; }

		public decimal Money { get; set; }

		public Guid Guid { get; set; }


		public override string ToString()
		{
			return string.Format("StringVal: {0}; IntVal: {1}; DtValue: {2}; Money: {3}; Guid: {4}",
				this.StringVal, this.IntVal, this.DtValue.ToString("yyyy-MM-dd HH:mm:ss"), this.Money.ToString("F4"), this.Guid);
		}


		public static TestSerializerModel Create(string stringVal, int intVal, DateTime dtValue, decimal money, Guid guid)
		{
			return new TestSerializerModel {
				StringVal = stringVal,
				IntVal = intVal,
				DtValue = dtValue,
				Money = money,
				Guid = guid
			};
		}
	}



	public class TestSerializerModel2
	{
		public string StringVal { get; set; }

		public int? IntVal { get; set; }

		public DateTime? DtValue { get; set; }

		public decimal? Money { get; set; }

		public Guid? Guid { get; set; }


		public override string ToString()
		{
			return string.Format("StringVal: {0}; IntVal: {1}; DtValue: {2}; Money: {3}; Guid: {4}",
				(this.StringVal == null ? "null" : this.StringVal),
				(this.IntVal.HasValue ? this.IntVal.Value.ToString() : "null"),
				(this.DtValue.HasValue ? this.DtValue.Value.ToString("yyyy-MM-dd HH:mm:ss") : "null"), 
				(this.Money.HasValue ? this.Money.Value.ToString("F4") : "null"),
				(this.Guid.HasValue ? this.Guid.Value.ToString() : "null")
				);
		}


		public static TestSerializerModel2 Create(string stringVal, int? intVal, DateTime? dtValue, decimal? money, Guid? guid)
		{
			return new TestSerializerModel2 {
				StringVal = stringVal,
				IntVal = intVal,
				DtValue = dtValue,
				Money = money,
				Guid = guid
			};
		}
	}



	public class TestSerializerModel3
	{
		public string StringVal { get; set; }

		public Category Category { get; set; }

		public Product Product { get; set; }

		public Customer Customer { get; set; }

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if( obj == null )
				return false;

			TestSerializerModel3 model = obj as TestSerializerModel3;
			if( model == null )
				return false;

			if( this.StringVal != model.StringVal )
				return false;

			if( this.Category.CategoryID != model.Category.CategoryID
					|| this.Category.CategoryName != model.Category.CategoryName )
				return false;

			if( this.Customer.Address != model.Customer.Address
					|| this.Customer.ContactName != model.Customer.ContactName
					|| this.Customer.CustomerID != model.Customer.CustomerID
					|| this.Customer.CustomerName != model.Customer.CustomerName
					|| this.Customer.PostalCode != model.Customer.PostalCode
					|| this.Customer.Tel != model.Customer.Tel )
				return false;


			if( this.Product.CategoryID != model.Product.CategoryID
					|| this.Product.ProductID != model.Product.ProductID
					|| this.Product.ProductName != model.Product.ProductName
					|| this.Product.Quantity != model.Product.Quantity
					|| this.Product.Remark != model.Product.Remark
					|| this.Product.Unit != model.Product.Unit
					|| this.Product.UnitPrice.ToString("F4") != model.Product.UnitPrice.ToString("F4") )
				return false;

			return true;
		}
	}
}
