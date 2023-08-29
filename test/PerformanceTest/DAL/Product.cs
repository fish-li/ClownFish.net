using System.ComponentModel.DataAnnotations.Schema;

namespace PerformanceTest.DAL;

[Table("products")]
[DbEntity(Alias = "products")]
public partial class Product : Entity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [DbColumn(PrimaryKey = true, Identity = true)]
    public virtual int ProductID { get; set; }

    public virtual string ProductName { get; set; }

    public virtual int CategoryID { get; set; }

    public virtual string Unit { get; set; }

    public virtual decimal UnitPrice { get; set; }

    public virtual string Remark { get; set; }

    public virtual int Quantity { get; set; }
}

