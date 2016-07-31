
// ClownFish.Data dynamically generate code
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Xml.Serialization;
using System.Linq;
using System.Text;
using ClownFish.Data;

//[assembly: ClownFish.Data.EntityProxyAssembly]
//[assembly: System.Reflection.AssemblyTitle("ClownFish.Data.ProxyGen")]
//[assembly: System.Reflection.AssemblyDescription("")]
//[assembly: System.Reflection.AssemblyConfiguration("")]
//[assembly: System.Reflection.AssemblyCompany("")]
//[assembly: System.Reflection.AssemblyProduct("ClownFish.Data.ProxyGen")]
//[assembly: System.Reflection.AssemblyCopyright("Copyright © ClownFish 2016")]
//[assembly: System.Reflection.AssemblyTrademark("")]
//[assembly: System.Reflection.AssemblyCulture("")]
//[assembly: System.Runtime.InteropServices.ComVisible(false)]
//[assembly: System.Runtime.InteropServices.Guid("bf90e507-2265-49e6-8539-a221e3f9832c")]
//[assembly: System.Reflection.AssemblyVersion("1.0.0.0")]
//[assembly: System.Reflection.AssemblyFileVersion("1.2016.731.1121")]

namespace ClownFish.Data.GeneratorCode
{
	[Serializable]
	public class Product_22504B0AA59DC41CEDC8925382100BEA_Proxy : ClownFish.Data.UnitTest.Models.Product, IEntityProxy
	{

		private bool[] _x_changeFlags = new bool[7];
		private ClownFish.Data.UnitTest.Models.Product _x_realEntity;

		Entity IEntityProxy.RealEntity { get { return _x_realEntity; } }
		private ClownFish.Data.UnitTest.Models.Product _X__Entity {
			get {
				if( _x_realEntity == null )
					_x_realEntity = new ClownFish.Data.UnitTest.Models.Product();
				return _x_realEntity;
			}
		}

		void IEntityProxy.Init(Entity entity)
		{
			_x_realEntity = (ClownFish.Data.UnitTest.Models.Product)entity;
		}

		void IEntityProxy.ClearChangeFlags()
		{
			for( int i = 0; i < _x_changeFlags.Length; i++ )
				_x_changeFlags[i] = false;
		}
		#region Properties

		public override int ProductID {
			get { return _x_realEntity.ProductID; }
			set { _x_changeFlags[0] = true; _X__Entity.ProductID = value; }
		}
		public override string ProductName {
			get { return _x_realEntity.ProductName; }
			set { _x_changeFlags[1] = true; _X__Entity.ProductName = value; }
		}
		public override int CategoryID {
			get { return _x_realEntity.CategoryID; }
			set { _x_changeFlags[2] = true; _X__Entity.CategoryID = value; }
		}
		public override string Unit {
			get { return _x_realEntity.Unit; }
			set { _x_changeFlags[3] = true; _X__Entity.Unit = value; }
		}
		public override decimal UnitPrice {
			get { return _x_realEntity.UnitPrice; }
			set { _x_changeFlags[4] = true; _X__Entity.UnitPrice = value; }
		}
		public override string Remark {
			get { return _x_realEntity.Remark; }
			set { _x_changeFlags[5] = true; _X__Entity.Remark = value; }
		}
		public override int Quantity {
			get { return _x_realEntity.Quantity; }
			set { _x_changeFlags[6] = true; _X__Entity.Quantity = value; }
		}
		#endregion

		string[] IEntityProxy.GetChangeNames()
		{
			List<string> list = new List<string>(7);
			if( _x_changeFlags[0] )
				list.Add("ProductID");
			if( _x_changeFlags[1] )
				list.Add("ProductName");
			if( _x_changeFlags[2] )
				list.Add("CategoryID");
			if( _x_changeFlags[3] )
				list.Add("Unit");
			if( _x_changeFlags[4] )
				list.Add("UnitPrice");
			if( _x_changeFlags[5] )
				list.Add("Remark");
			if( _x_changeFlags[6] )
				list.Add("Quantity");
			return list.ToArray();
		}
		object[] IEntityProxy.GetChangeValues()
		{
			List<object> list = new List<object>(7);
			if( _x_changeFlags[0] )
				list.Add(this.ProductID);
			if( _x_changeFlags[1] )
				list.Add(this.ProductName);
			if( _x_changeFlags[2] )
				list.Add(this.CategoryID);
			if( _x_changeFlags[3] )
				list.Add(this.Unit);
			if( _x_changeFlags[4] )
				list.Add(this.UnitPrice);
			if( _x_changeFlags[5] )
				list.Add(this.Remark);
			if( _x_changeFlags[6] )
				list.Add(this.Quantity);
			return list.ToArray();
		}
		Tuple<string, object> IEntityProxy.GetRowKey()
		{
			return new Tuple<string, object>("ProductID", this.ProductID);
		}

		[XmlAttribute]
		public bool[] XEntityDataChangeFlags { get { return _x_changeFlags; } set { _x_changeFlags = value; } }
	}
	[EntityAddition(ProxyType = typeof(Product_22504B0AA59DC41CEDC8925382100BEA_Proxy))]
	public class Product_22504B0AA59DC41CEDC8925382100BEA_Loader : BaseDataLoader<ClownFish.Data.UnitTest.Models.Product>, IDataLoader<ClownFish.Data.UnitTest.Models.Product>
	{

		public override int[] CreateIndex(object dataSource)
		{
			return DataLoaderHelper.CreateNameMapIndex(dataSource, 7
		   , new KeyValuePair<int, string>(0, "ProductID")
		   , new KeyValuePair<int, string>(1, "ProductName")
		   , new KeyValuePair<int, string>(2, "CategoryID")
		   , new KeyValuePair<int, string>(3, "Unit")
		   , new KeyValuePair<int, string>(4, "UnitPrice")
		   , new KeyValuePair<int, string>(5, "Remark")
		   , new KeyValuePair<int, string>(6, "Quantity")
		   );
		}
		public override void LoadFromDataReader(DbDataReader reader, int[] colIndex, ClownFish.Data.UnitTest.Models.Product m)
		{
			if( colIndex[0] >= 0 ) {
				m.ProductID = reader.GetInt32(colIndex[0]);
			}
			if( colIndex[1] >= 0 ) {

				object val = reader.GetValue(colIndex[1]);
				if( val != null && DBNull.Value.Equals(val) == false )
					m.ProductName = (string)(val);
			}
			if( colIndex[2] >= 0 ) {
				m.CategoryID = reader.GetInt32(colIndex[2]);
			}
			if( colIndex[3] >= 0 ) {

				object val = reader.GetValue(colIndex[3]);
				if( val != null && DBNull.Value.Equals(val) == false )
					m.Unit = (string)(val);
			}
			if( colIndex[4] >= 0 ) {
				m.UnitPrice = reader.GetDecimal(colIndex[4]);
			}
			if( colIndex[5] >= 0 ) {

				object val = reader.GetValue(colIndex[5]);
				if( val != null && DBNull.Value.Equals(val) == false )
					m.Remark = (string)(val);
			}
			if( colIndex[6] >= 0 ) {
				m.Quantity = reader.GetInt32(colIndex[6]);
			}
		}
		public override void LoadFromDataRow(DataRow row, int[] colIndex, ClownFish.Data.UnitTest.Models.Product m)
		{
			if( colIndex[0] >= 0 ) {
				m.ProductID = (int)(row[colIndex[0]]);
			}
			if( colIndex[1] >= 0 ) {

				object val = row[colIndex[1]];
				if( val != null && DBNull.Value.Equals(val) == false )
					m.ProductName = (string)(val);
			}
			if( colIndex[2] >= 0 ) {
				m.CategoryID = (int)(row[colIndex[2]]);
			}
			if( colIndex[3] >= 0 ) {

				object val = row[colIndex[3]];
				if( val != null && DBNull.Value.Equals(val) == false )
					m.Unit = (string)(val);
			}
			if( colIndex[4] >= 0 ) {
				m.UnitPrice = (decimal)(row[colIndex[4]]);
			}
			if( colIndex[5] >= 0 ) {

				object val = row[colIndex[5]];
				if( val != null && DBNull.Value.Equals(val) == false )
					m.Remark = (string)(val);
			}
			if( colIndex[6] >= 0 ) {
				m.Quantity = (int)(row[colIndex[6]]);
			}
		}
	}
}