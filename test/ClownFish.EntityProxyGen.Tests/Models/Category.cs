using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ClownFish.UnitTest.Data.Models;

[DbEntity(Alias = "Categories")]
public partial class Category : Entity
{
    /// <summary>
    /// CategoryID
    /// <summary>
    [DbColumn(Alias = "CategoryID", PrimaryKey = true, Identity = true)]
    public virtual int CategoryID { get; set; }

    /// <summary>
    /// CategoryName
    /// <summary>
    [DbColumn(Alias = "CategoryName")]
    public virtual string CategoryName { get; set; }

}
