namespace ClownFish.UnitTest.Http.Pipleline.Test;

[AttributeUsage(AttributeTargets.All)]
public class TestMvcAttribute : Attribute
{
    public string X1 { get; set; }
}
