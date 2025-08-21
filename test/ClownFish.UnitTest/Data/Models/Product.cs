namespace ClownFish.UnitTest.Data.Models;

[Serializable]
[DbEntity(Alias = "Products")]
public class Product : Entity
{
    [DbColumn(PrimaryKey = true, Identity = true)]
    public virtual int ProductID { get; set; }
    public virtual string ProductName { get; set; }
    public virtual int CategoryID { get; set; }

    [DbColumn(DefaultValue = "只")]
    public virtual string Unit { get; set; }

    [DbColumn(DefaultValue = 147.36)]
    public virtual decimal UnitPrice { get; set; }

    [DbColumn(DefaultValue = "xxx")]
    public virtual string Remark { get; set; }
    public virtual int Quantity { get; set; }
}
