using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Data;

namespace ClownFish.UnitTest.Data._DEMO
{
    class DEMO_10_Fiddler_Plugin
    {
        // 为了方便在Fiddler查看请求时，顺利查看某个请求的SQL执行情况，
        // 可以安装一个与 ClownFish.Data 配对使用的Fiddler插件，源码地址：
        // https://github.com/fish-li/ClownFish.tools/tree/master/src/ClownFish.FiddlerPulgin
        // 编译后，将得到的DLL复制到Fiddler安装目录下的Scripts即可
        // 例如，如果安装的是 Fiddler5， 即 C:\Users\[你的登录名]\AppData\Local\Programs\Fiddler\Scripts 



        // 同时需要在站点的 web.config 中注册一个 Module
        // <add name="FiddlerProfilerModule" type="ClownFish.WebApp.Profiler.FiddlerProfilerModule, ClownFish.WebApp.Profiler" preCondition="integratedMode" />

        // https://github.com/fish-li/ClownFish.Tucao  这个项目已经配置好了，代码拉到本机编译部署即可查看。



        // 最终可以看到的效果：
        // http://note.youdao.com/noteshare?id=3bc614e60011f46ae2fdeadb235e2dca&sub=69CB44521FB7493EA5B338E1AAD59F77

    }
}
