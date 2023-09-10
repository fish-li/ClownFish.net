global using ClownFish.UnitTest._Common;
global using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Web.UnitTest;

[TestClass]
public class Initializer
{
    [AssemblyInitialize]
    public static void InitRuntime(TestContext context)
    {
        AsmHelper.SetEntryAssembly(typeof(Initializer).Assembly);
        EnvironmentVariables.Set("MySqlClientProviderSupport", "3");
        EnvironmentVariables.Set("ASPNETCORE_ENVIRONMENT", "Development");

        System.Environment.CurrentDirectory = Path.GetDirectoryName(typeof(Initializer).Assembly.Location);

        ClownFishInit.InitBase();
        ClownFish.Web.Security.Auth.AuthenticationManager.InitAsDefault();
    }



    [AssemblyCleanup()]
    public static void AssemblyCleanup()
    {
        // 等待 HttpWriter的操作
        System.Threading.Thread.Sleep(2000);
    }


}
