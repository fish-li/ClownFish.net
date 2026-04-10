using System;
using System.Collections.Generic;
using System.Text;

namespace ClownFish.UnitTest.Data.Models;

[DbEntity]
public class Model11 : Entity
{
    public virtual int IntValue { get; set; }
    public virtual string StrValue { get; set; }

    public int IntValue2;

    public string StrValue2;
}


public class Model12 : Entity
{
    public virtual int IntValue { get; set; }
    public virtual string StrValue { get; set; }
}

