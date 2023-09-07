namespace ClownFish.UnitTest.Data.Models;

[Serializable]
[DbEntity(Alias = "TestDateTimeTable")]
public partial class TestDateTimeTable : Entity
{
    /// <summary>
    /// Rid
    /// <summary>
    [DbColumn(Alias = "Rid", PrimaryKey = true, Identity = true)]
    public virtual int Rid { get; set; }

    /// <summary>
    /// Time1
    /// <summary>
    [DbColumn(Alias = "Time1")]
    public virtual DateTime? Time1 { get; set; }        // NULL

    /// <summary>
    /// Time2
    /// <summary>
    [DbColumn(Alias = "Time2")]
    public virtual DateTime Time2 { get; set; }         // Zero

    /// <summary>
    /// Time3
    /// <summary>
    [DbColumn(Alias = "Time3")]
    public virtual DateTime Time3 { get; set; }         // normal DatetTime

}
