using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// 有关程序集的常规信息通过下列属性集
// 控制。更改这些属性值可修改
// 与程序集关联的信息。
[assembly: AssemblyTitle("DEMO.Controllers")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("Demo.Controller")]
[assembly: AssemblyCopyright("Copyright ©  2012")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// 将 ComVisible 设置为 false 使此程序集中的类型
// 对 COM 组件不可见。如果需要从 COM 访问此程序集中的类型，
// 则将该类型上的 ComVisible 属性设置为 true。
[assembly: ComVisible(false)]

// 如果此项目向 COM 公开，则下列 GUID 用于类型库的 ID
[assembly: Guid("68502006-c804-4f90-aef7-7ace4656eb9b")]

// 程序集的版本信息由下面四个值组成:
//
//      主版本
//      次版本 
//      内部版本号
//      修订号
//
// 可以指定所有这些值，也可以使用“内部版本号”和“修订号”的默认值，
// 方法是按如下所示使用“*”:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]





// #######################################################################
// 请注意下面三个Attribute
// #######################################################################

// 指定这是一个包含了Controller的程序集，如果不指定，ClownFish.Web框架将不搜索这个程序中的Controller（Action）
[assembly: ClownFish.Web.ControllerAssembly]


// 下面定义了一个命名空间的别名，如果Controller包含在多个命名空间，请分别定义多个对应的别名
// 说明：命名空间别名不是必须的，使用它只是为了让URL更短小而已，也可以不使用这个特性，
//      如果不使用命名空间别名，就需要指定命名空间的名称（完整路径）

[assembly: ClownFish.Web.NamespaceMap("DEMO.Controllers.Services", "ns")]
// URL用法： http://www.fish-web-demo.com/api/ns/Demo1/TestGuid2.aspx

[assembly: ClownFish.Web.NamespaceMap("DEMO.Controllers.AjaxPK", "pk")]
// URL用法： http://www.fish-web-demo.com/api/pk/AjaxPK/Add.aspx

