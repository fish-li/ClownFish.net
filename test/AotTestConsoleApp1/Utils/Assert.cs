using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AotTestConsoleApp1.Utils;
internal static class Assert
{
    public static void IsNull(object obj)
    {
        if( obj == null )
            return;

        throw new ValidationException2("IsNull 断言失败");
    }

    public static void IsNotNull(object obj)
    {
        if( obj != null )
            return;

        throw new ValidationException2("IsNotNull 断言失败");
    }

    public static void IsTrue(bool bb)
    {
        if( bb )
            return;

        throw new ValidationException2("IsTrue 断言失败");
    }

    public static void AreEqual(string a, string b)
    {
        if( a == b )
            return;

        throw new ValidationException2($"IsTrue 断言失败，期望值：<{a}>， 实际值：<{b}>");
    }

    public static void AreEqual(int a, int b)
    {
        if( a == b )
            return;

        throw new ValidationException2($"IsTrue 断言失败，期望值：<{a}>， 实际值：<{b}>");
    }

    public static void IsInstanceOfType(object obj, Type type)
    {
        if( obj == null )
            throw new ValidationException2("IsInstanceOfType 断言失败，obj is null.");

        if( type == null )
            throw new ValidationException2("IsInstanceOfType 断言失败，type is null.");

        if( obj.GetType().IsSubclassOf(type) )
            return;

        throw new ValidationException2($"IsInstanceOfType 断言失败，期望类型：<{type.FullName}>， 实际类型：<{obj.GetType().FullName}>");
    }
}
