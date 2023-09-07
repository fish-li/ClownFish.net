namespace ClownFish.UnitTest.Base.Reflection;

[TestClass]
public class ReflectionTest
{
    [TestMethod]
    public void Test_CallDelegateSetTargetNull()
    {
        // 这个测试用例是为了检测能否避开一个 CLR BUG

        // BUG描述如下：
        // 1，首先要定义一个虚属性，例如：public virtual int InputA { get; set; }
        // 2、当用 FastGetValue 之类方法生成委托时，第一次传递有效的 target
        // 3、第二次调用时，传递 taget = null
        // 4，在第 2 轮调用时，将会出现 System.AccessViolationException:“Attempted to read or write protected memory. This is often an indication that other memory is corrupt.”

        // 补救方法：在调用委托前检查 target is null，如果成立就主动抛出 ArgumentNullException，防止调用委托时传递 null 而造成 AccessViolationException

        PropertyInfo property = typeof(DataObject).GetProperty(nameof(DataObject.InputA), BindingFlags.Instance | BindingFlags.Public);

        DataObject data = new DataObject();

        for( int i = 0; i < 3; i++ ) {
            try {
                object value1 = property.FastGetValue(data);
                object value2 = property.FastGetValue(null);
            }
            catch( ArgumentNullException ) {

            }
        }
    }



    [TestMethod]
    public void Test_Error()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            _ = new CommonInvokerWrapper(null);
        });


        MethodInfo action0 = typeof(InstanceObject).GetMethod(nameof(InstanceObject.Action0), BindingFlags.Instance | BindingFlags.Public);
        ActionWrapper<InstanceObject> wrapper0 = new ActionWrapper<InstanceObject>();
        InstanceObject test = new InstanceObject(123);

        MyAssert.IsError<InvalidOperationException>(() => {
            wrapper0.Invoke(test, null);
        });
    }

    


    

}
