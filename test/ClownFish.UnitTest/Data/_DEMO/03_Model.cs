namespace ClownFish.UnitTest.Data._DEMO;

class DEMO_3_Model
{
}


// 定义实体
// 【建议】实体从 Entity 继承

public class Model11 : Entity
{
    public virtual int IntValue { get; set; }
    public virtual string StrValue { get; set; }

    // 只支持属性，不支持字段
    // 运行时生成继承类，重写虚属性生成属性修改通知，记录属性赋值
}
