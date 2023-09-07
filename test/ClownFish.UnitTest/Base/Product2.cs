namespace ClownFish.UnitTest.Base;

[Serializable]
public class Product2
{
    public int ProductID { get; set; }
    public string ProductName { get; set; }
    public int CategoryID { get; set; }
    public string Unit { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }

    public string Remark { get; set; }

    public ClownFish.Base.Xml.XmlCdata LongText { get; set; }

    public override string ToString()
    {
        return string.Format("id={0};name={1}", this.ProductID, this.ProductName);
    }


    public static Product2 CreateByRandomData()
    {
        DateTime dt = DateTime.Now;

        Product2 p = new Product2();
        p.ProductID = dt.Year;
        p.ProductName = Guid.NewGuid().ToString();
        p.CategoryID = dt.Month;
        p.Unit = dt.Day.ToString();
        p.UnitPrice = dt.Hour;
        p.Remark = Guid.NewGuid().ToString();
        p.LongText = dt.ToTimeString();
        return p;
    }


    public static Product2 CreateByFixedData()
    {
        Product2 p = new Product2();
        p.ProductID = 123;
        p.ProductName = "MSDN Library";
        p.CategoryID = 8;
        p.Unit = "个";
        p.UnitPrice = 56.34m;
        p.Remark = "现在可以对目录和索引使用预定义的筛选器，而为搜索使用一个不同的自定义筛选器。";
        p.LongText = "保存搜索查询   您可以保存帮助搜索查询，从而能够在需要时重新运行同一搜索查询。";
        return p;
    }


    public bool IsEqual(Product2 p)
    {
        if( p == null )
            return false;

        return this.ProductID == p.ProductID
            && this.ProductName == p.ProductName
            && this.CategoryID == p.CategoryID
            && this.Unit == p.Unit
            && (this.UnitPrice - p.UnitPrice < 0.0001m)
            && this.Remark == p.Remark
            && this.LongText == p.LongText;
    }
}
