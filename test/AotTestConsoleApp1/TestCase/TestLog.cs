using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AotTestConsoleApp1.TestCase;
internal static class TestLog
{
    public static bool Filter(object data)
    {
        if( data is OprLog oprlog )
            return oprlog.Status != 147258000;

        if( data is InvokeLog invokeLog )
            return invokeLog.ActionType != 147258000;

        return true;
    }

    public static async Task Run()
    {
        OprLog log1 = new OprLog {
            OprId = "id_1",
            Action = "run",
            OprName = "Name",
        };

        LogHelper.Write(log1);

        InvokeLog log2 = new InvokeLog {
            ActionType = 100,
            AppName = "app2",
            HasError = 1,
            IsSlow = 1
        };

        LogHelper.Write(log2);

        await Task.Delay(1000);
    }

}
