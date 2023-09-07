namespace ClownFish.UnitTest.Data._DEMO;

class DEMO_9_Extend
{
    // ClownFish.Data提供以下扩展事件

    /*        
    class DbContextEvent {
        event OnConnectionOpened;
        event OnBeforeExecute;
        event OnAfterExecute;
        event OnCommited;
    }
     */


    // 利用这些事件，可以实现：

    // 1、数据访问的监控，
    // 例如：https://github.com/fish-li/ClownFish.tools/blob/master/src/ClownFish.WebApp.Profiler/DataLayerEventSubscriber.cs
    // 具体效果：http://note.youdao.com/noteshare?id=3bc614e60011f46ae2fdeadb235e2dca&sub=69CB44521FB7493EA5B338E1AAD59F77

    // 2、数据访问的日志，
    // 例如：https://github.com/fish-li/Snakehead.Cbdmt/blob/master/ext/ClownFish.DataLog.Extension/ClownFishDataEventSubscriber.cs

}
