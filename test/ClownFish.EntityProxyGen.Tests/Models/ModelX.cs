using System;
using System.Collections.Generic;
using System.Text;

namespace ClownFish.UnitTest.Data.Models;


[DbEntity(Alias = "TestTable1")]
public class ModelX : Entity
{
    [DbColumn(PrimaryKey = true, Alias = "rid")]
    public virtual int RowId { get; set; }              // changeflag: 0
    [DbColumn(Alias = "intA")]
    public virtual int IntField { get; set; }           // changeflag: 1
    [DbColumn(Alias = "timeA")]
    public virtual DateTime TimeField { get; set; }     // changeflag: 2
    [DbColumn(Alias = "moneyA")]
    public virtual decimal MoneyField { get; set; }     // changeflag: 3
    [DbColumn(Alias = "stringA")]
    public virtual string StringField { get; set; }     // changeflag: 4
    [DbColumn(Alias = "boolA")]
    public bool BoolField { get; set; }                 // 不是虚属性
    [DbColumn(Alias = "guidA")]
    public virtual Guid GuidField { get; set; }         // changeflag: 6
    [DbColumn(Alias = "intB")]
    public virtual int? IntNullField { get; set; }      // changeflag: 7
    [DbColumn(Alias = "moneyB")]
    public virtual decimal? MoneyNullField { get; set; }// changeflag: 8
    [DbColumn(Alias = "guidB")]
    public Guid? GuidNullField { get; set; }            // 不是虚属性

    public List<int> ListInt { get; set; }            // 不受支持
    public int[] IntAtrray { get; set; }                // 不受支持

    public ClownFish.Data.ConnectionInfo ConnectionInfo { get; set; }  // 不受支持

    [DbColumn(Alias = "shortB")]
    public virtual short? ShortField { get; set; }
    [DbColumn(Alias = "charA")]
    public virtual char Char1 { get; set; }
    [DbColumn(Alias = "charB")]
    public virtual char? Char2 { get; set; }
    [DbColumn(Alias = "img")]
    public virtual byte[] Image { get; set; }

    [DbColumn(Alias = "text1", Ignore = true)]
    public virtual string Text1 { get; set; }

    protected virtual string Text2 { get; set; }

    [DbColumn(Alias = "g2")]
    public virtual Guid AutoGuid { get; set; }
    [DbColumn(Alias = "ts")]
    public virtual byte[] TimeStamp { get; set; }

    
}