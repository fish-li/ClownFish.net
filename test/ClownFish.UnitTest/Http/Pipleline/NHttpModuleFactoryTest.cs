namespace ClownFish.UnitTest.Http.Pipleline;

[TestClass]
public class NHttpModuleFactoryTest
{
    [TestMethod]
    public void Test_Error()
    {
        MyAssert.IsError<ArgumentNullException>(()=> {
            NHttpModuleFactory.RegisterModule(null);
        });

        MyAssert.IsError<ArgumentOutOfRangeException>(() => {
            NHttpModuleFactory.RegisterModule(typeof(string));
        });

        MyAssert.IsError<ArgumentException>(() => {
            NHttpModuleFactory.RegisterModule(typeof(XModule));
        });
    }


    public class XModule : NHttpModule
    {
        public XModule(string xx)
        {

        }
    }
}
