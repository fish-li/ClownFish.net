using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.UnitTest.Data.Command;
[TestClass]
public class CommandExtensionsTest
{
    [TestMethod]
    public async Task Test_error()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            CPQuery query = null;
            _ = query.ExportToNdJson(100, "aaa.txt");
        });

        await MyAssert.IsErrorAsync<ArgumentNullException>(async () => {
            CPQuery query = null;
            _ = await query.ExportToNdJsonAsync(100, "aaa.txt");
        });


        StringBuilder sb = new StringBuilder();
        StringWriter writer = new StringWriter(sb);

        MyAssert.IsError<ArgumentNullException>(() => {
            CPQuery query = null;
            _ = query.ExportToNdJson(100, writer);
        });

        await MyAssert.IsErrorAsync<ArgumentNullException>(async () => {
            CPQuery query = null;
            _ = await query.ExportToNdJsonAsync(100, writer);
        });

    }
}
