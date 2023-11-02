using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.UnitTest.Tasks;

#if NETCOREAPP
using ClownFish.Tasks;

[TestClass]
public class NormalTaskTest
{
    [TestMethod]
    public void Test_1()
    {
        NormalTaskExt1 t1 = new NormalTaskExt1();

        t1.Run();

        MyAssert.IsError<InvalidOperationException>(() => {
            t1.Run();
        });
        
    }
}


public class NormalTaskExt1 : NormalTask
{
    public override bool EnableLog => false;

    private int _count = 0;

    public override void Execute()
    {
        _count++;

        if( _count % 2 == 1 )
            Console.WriteLine("OK");
        else
            throw new InvalidOperationException();
    }
}


#endif
