namespace ClownFish.UnitTest.Data.Models;

/// <summary>
/// 这个实体类型用于验证[DbColumn(Alias="xxxxxxxxxx")]
/// </summary>
public class Product2 : Entity
{
    [DbColumn(Alias = "ProductID", PrimaryKey = true)]
    public virtual int PId { get; set; }

    [DbColumn(Alias = "ProductName")]
    public virtual string PName { get; set; }

    [DbColumn(Alias = "CategoryID")]
    public virtual int CID { get; set; }

    [DbColumn(Alias = "Unit")]
    public virtual string Unt { get; set; }

    [DbColumn(Alias = "UnitPrice")]
    public virtual decimal UPrice { get; set; }

    [DbColumn(Alias = "Remark")]
    public virtual string Remark2 { get; set; }

    [DbColumn(Alias = "Quantity")]
    public virtual int Quantity2 { get; set; }
}
