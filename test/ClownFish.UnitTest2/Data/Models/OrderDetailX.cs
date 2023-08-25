using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Data;

namespace ClownFish.UnitTest.Data.Models
{
    //[DbEntity(Alias = "OrderDetails1")]
    public partial class OrderDetailX1
    {
        [DbColumn(PrimaryKey = true)]
        public int OrderID { get; set; }

        [DbColumn(PrimaryKey = true)]
        public int ProductID { get; set; }

        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }
    }


    //[DbEntity(Alias = "OrderDetails2")]
    public partial class OrderDetailX2 : Entity
    {
        [DbColumn(Identity = true)]
        public int OrderID { get; set; }

        [DbColumn(Identity = true)]
        public int ProductID { get; set; }

        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }

        [DbColumn(Ignore = true)]
        public int X1 { get; set; }

        public int X2 {
            set => this.X1 = value;
        }

        public int X3 => this.X1;

        public int this[string x] {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
    }



    [DbEntity(Alias = "OrderDetail_x3")]
    public class OrderDetailX3 : Entity
    {
        [DbColumn(PrimaryKey = true)]
        public int RowId { get; set; }

        [DbColumn(Identity = true)]
        public int NewID { get; set; }

        [DbColumn(DefaultValue = 2.34)]
        public decimal UnitPrice { get; set; }

        [DbColumn(DefaultValue = 123, Alias = "Quantity2")]
        public int Quantity { get; set; }

        [DbColumn(DefaultValue = "abcd", Alias = null)]
        public string Remark { get; set; }

    }


    [DbEntity(Alias = "")]
    public class OrderDetailX4 : Entity
    {

    }


    public abstract class OrderDetailX5 : Entity
    {

    }

    public sealed class OrderDetailX6 : Entity
    {

    }
}
