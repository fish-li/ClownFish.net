namespace ClownFish.Base;

/// <summary>
/// 辅助测试的工具类
/// </summary>
internal static class TestHelper
{
    private static Exception s_exceptionForTest = null;

    /// <summary>
    /// 为测试强制设置一个异常，然后在调用TryThrowException()时将会抛出，
    /// 由于这个属性仅仅用于测试环境，因此不考虑线程安全问题
    /// </summary>
    /// <param name="ex"></param>
    public static void SetException(Exception ex)
    {
        s_exceptionForTest = ex;
    }

    /// <summary>
    /// 调用这个方法可以模拟意外的异常发生，用于检验catch的代码是否能正确工作。
    /// 抛出 ExceptionForTest 指定的异常，并将ExceptionForTest设置为NULL，
    /// 如果 ExceptionForTest 为NULL，将忽略本次调用
    /// </summary>
    public static void TryThrowException()
    {
        if( s_exceptionForTest != null ) {
            Exception ex = s_exceptionForTest;

            // 确保只触发一次
            s_exceptionForTest = null;

            throw ex;
        }
    }


}

