namespace ClownFish.UnitTest._Common;

internal static class ExceptionHelper
{
    internal static Exception CreateException()
    {
        try {
            throw new MessageException("一个用于测试的异常");
        }
        catch( Exception ex ) {
            return ex;
        }
    }

    internal static Exception CreateException(string message)
    {
        try {
            try {
                throw new MessageException("这是个内部异常");
            }
            catch( Exception ex ) {
                throw new InvalidOperationException(message, ex);
            }
        }
        catch( Exception ex2 ) {
            return ex2;
        }
    }


    public static string ExecuteActionReturnErrorMessage(Action action)
    {
        try {
            action();
        }
        catch( Exception ex ) {
            return ex.Message;
        }


        throw new InternalTestFailureException("异常没有出现！");
    }

    public async static Task<string> ExecuteActionReturnErrorMessage(Func<Task> action)
    {
        try {
            await action();
        }
        catch( Exception ex ) {
            return ex.Message;
        }


        throw new InternalTestFailureException("异常没有出现！");
    }


}
