using ClownFish.Web.Aspnetcore;

namespace ClownFish.Tracing;

public static class TracingInitializer
{
    private static bool s_inited = false;
    private static readonly object s_lock = new object();

    public static void Init()
    {
        if( s_inited == false ) {
            lock( s_lock ) {
                if( s_inited == false ) {

                    Init0();
                    s_inited = true;
                }
            }
        }
    }


    private static void Init0()
    {
        if( LoggingOptions.TracingEnabled == false )
            return;


        TracingUtils.Init();

        AspnetcoreLogger.Init();
    }

}
