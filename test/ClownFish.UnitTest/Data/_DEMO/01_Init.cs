namespace ClownFish.UnitTest.Data._DEMO;

class DEMO_1_Init
{
    // 为了方便地使用ClownFish.Data，建议在程序启动的地方执行初始化
    // 对于 WinForm , Console 类型的项目中，可以在 Program.cs 中调用
    // 对于 ASP.NET 类型的项目中，可以在 App_Start方法中调用


    private void Init()
    {
        // ########################################
        // ClownFish.Data 初始化 （都是可选步骤）：
        // ########################################

         ClownFish.Data.Initializer.Instance
                        // 用指定的配置文件初始化连接字符串
                        // 如果参数为NULL，将会从 app.config or web.config 中读取连接的配置信息
                        .InitConnection()

                        // 加载 XmlCommand （如果不使用XmlCommand，可以忽略这个步骤）
                        .LoadXmlCommandFromDirectory(/* 不指定参数，接受XmlCommand规范的默认目录  */)

                        // 为数据实体生成代理类型，并加载已生成的实体代理程序集
                        .CompileAllEntityProxy(@"xxx.dll");



        // 关于实体代理类型的说明
        // 代理类的代码可参考文件：AutoCode1.cs
        // 主要用途有二块：1、记录属性的修改（xxxx_Proxy），2、快速加载实体（xxxx_Loader）

        // 有二种方法可以得到实体代理类型：
        // 1、像上面那样调用 CompileAllEntityProxy()，将在运行时生成临时程序集（占用运行时时间）
        // 2、先调用命令行工具 ClownFish.Data.ProxyGen 生成程序集文件，启动时再调用 CompileAllEntityProxy() 加载

        // 对于对性能没有苛求的项目，也可以不生成实体代理，那么ClownFish.Data会在运行时采用反射的方式来处理
        // 如果不生成代理类型，就不能以修改属性方式做CUD

    }


}
