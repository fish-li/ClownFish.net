using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Base.UnitTest._DEMO
{
    class ObjectFactory_DEMO
    {
        // ObjectFactory 是什么？ 有什么作用？
        // =================================================

        // ObjectFactory 可用于为代码提供扩展性。

        // 它提供二种机制，用于在不修改代码的前提下实现扩展：
        // 1、运行时用继承类来替代原类型，类似于IOC的类型容器。
        // 2、允许类型的事件订阅不写死在代码中，在创建类型的实例时，自动实现事件订阅。



        // 第一种机制示例：

        // ControllerRecognizer recognizer = ObjectFactory.New<ControllerRecognizer>();

        // 这段代码在 https://github.com/fish-li/ClownFish.net/blob/master/src/ClownFish.Web/Reflection/MetadataCache.cs
        // 由于我没有直接写成 ControllerRecognizer recognizer = new ControllerRecognizer();
        // 所以，在其它的项目中，可以扩展ControllerRecognizer这个类型，实现其它的行为，

        // 可以查看这里：https://github.com/fish-li/ClownFish.Tucao/blob/master/src/web-async/ClownFish.Extend/ControllerRecognizerExt.cs
        // 这个定义了新类型：public sealed class ControllerRecognizerExt : ControllerRecognizer

        // 所以，最终 ObjectFactory.New<ControllerRecognizer>() 返回的是 ControllerRecognizerExt
        // 通过这种方法得到的好处是：框架本身得到的扩展。




        // 第二种机制示例：

        // https://github.com/fish-li/ClownFish.net/blob/master/src/ClownFish.Data/Command/EventManager.cs
        // 在 ClownFish.Data 中，我为数据访问提供了一系列的事件，注意，这些事件并不是【静态】的。

        // 按照标准的 .NET 写法，我必须在使用时订阅，类似下面这种写法：
        // EventManager em = new EventManager();
        // em.ConnectionOpened += .....;

        // 只有这样，当EventManager在触发事件时，事件处理器才能被调用。
        // 如果还是不能理解，在 WinForm 的窗体中，放几个控件上去，为控件增加事件处理器就明白了：你只能在那个表单中订阅需要的事件。
        // =================================================
        // 你不能在其它的程序集中为动态创建的对象订阅事件！
        // =================================================

        // 然而，你可以看看这个代码文件：
        // https://github.com/fish-li/Snakehead.Cbdmt/blob/master/ext/ClownFish.DataLog.Extension/ClownFishDataEventSubscriber.cs
        // 我在另一个项目中，订阅了EventManager的事件

        // 最终ClownFish.Data在运行时，所有触发的事件，都会被ClownFishDataEventSubscriber响应。
        
        // 这里要说明二点：
        // 1、这个效果其实是和静态事件差不多的，但是实例事件是不允许在实例之部订阅的。 （听起来有点绕，没办法！）
        // 2、既然静态事件可以，为什么不直接使用静态事件？ 答：因为静态事件需要用静态方法来订阅，状态的维持会比较复杂。


    }
}
