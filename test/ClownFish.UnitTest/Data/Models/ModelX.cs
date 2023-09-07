namespace ClownFish.UnitTest.Data.Models;

[Serializable]
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

    public ConnectionInfo ConnectionInfo { get; set; }  // 不受支持

    [DbColumn(Alias = "shortB")]
    public virtual short? ShortField { get; set; }
    [DbColumn(Alias = "charA")]
    public virtual char Char1 { get; set; }
    [DbColumn(Alias = "charB")]
    public virtual char? Char2 { get; set; }
    [DbColumn(Alias = "img")]
    public virtual byte[] Image { get; set; }
    [DbColumn(Alias = "g2")]
    public virtual Guid AutoGuid { get; set; }
    [DbColumn(Alias = "ts")]
    public virtual byte[] TimeStamp { get; set; }
}


[Serializable]
public class Model1Proxy : ModelX, IEntityProxy
{
#pragma warning disable IDE0044 // 添加只读修饰符
    private bool[] _x_changeFlags = new bool[7];  // 7 个 virtual属性
#pragma warning restore IDE0044 // 添加只读修饰符
    public bool[] ChangeFlags { get { return _x_changeFlags; } }


    private DbContext _context;
    private ModelX _x_realEntity = null;

    public Entity InnerEntity { get { return _x_realEntity; } }

    public DbContext DbContext { get { return _context; } }
    

    public void ClearChangeFlags()
    {
        for( int i = 0; i < _x_changeFlags.Length; i++ )
            _x_changeFlags[i] = false;  // 清除修改标记
    }

    public void Init(DbContext dbContext, Entity entity)
    {
        _context = dbContext;
        _x_realEntity = (ModelX)entity;
    }

    public override int IntField {
        get {
            return _x_realEntity.IntField;
        }
        set {
            // 为了能识别【零值】，只要调用 set 就认为是修改
            _x_changeFlags[0] = true;
            _x_realEntity.IntField = value;
        }
    }

    public override int? IntNullField {
        get {
            return _x_realEntity.IntNullField;
        }

        set {
            _x_changeFlags[5] = true;
            _x_realEntity.IntNullField = value;
        }
    }

    public IReadOnlyList<string> GetChangeNames()
    {
        List<string> list = new List<string>();
        if( _x_changeFlags[0] )
            list.Add("IntField");

        if( _x_changeFlags[6] )
            list.Add("MoneyNullField");

        return list;
    }

    public IReadOnlyList<object> GetChangeValues()
    {
        List<object> list = new List<object>();

        if( _x_changeFlags[0] )
            list.Add(this.IntField);

        if( _x_changeFlags[6] ) {
            if( this.MoneyNullField.HasValue )
                list.Add(this.MoneyNullField.Value);
            else
                list.Add(null);
        }

        return list;
    }


    public FieldNvObject GetRowKey()
    {
        return new FieldNvObject("rid", this.RowId);
    }


}

public class Model1DataLoader : IDataLoader<ModelX>
{
    public ModelX ToSingle(DataRow row)
    {
        throw new NotImplementedException();
    }

    public ModelX ToSingle(DbDataReader reader)
    {
        throw new NotImplementedException();
    }

    public List<ModelX> ToList(DataTable table)
    {
        throw new NotImplementedException();
    }

    public List<ModelX> ToList(DbDataReader reader)
    {
        throw new NotImplementedException();
    }
}
