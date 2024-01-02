namespace ClownFish.Base;

/// <summary>
/// 处理【应用程序模块】的工具类
/// </summary>
public static class AppPartUtils
{
    private static List<Assembly> s_list;
    private static readonly object s_syncLock = new object();

    /// <summary>
    /// 获取所有的【应用程序模块】清单
    /// </summary>
    /// <returns></returns>
    public static List<Assembly> GetApplicationPartAsmList()
    {
        if( s_list == null ) {
            lock( s_syncLock ) {
                if( s_list == null ) {

                    List<Assembly> list = AsmHelper.GetAssemblyList<ApplicationPartAssemblyAttribute>();
                    Assembly exeAsm = AsmHelper.GetEntryAssembly();

                    if( list.Contains(exeAsm) == false )
                        list.Add(exeAsm);

                    // 允许排除一些应用程序模块
                    // 例如：一些 AllInOne 项目，在某个部署环境下可能就需要禁用其中的个另模块
                    string ignores = LocalSettings.GetSetting("ClownFish_IgnoreAppParts");
                    if( ignores.HasValue() ) {
                        string[] names = ignores.ToArray2();
                        s_list = list.Where(x => names.Contains(x.GetName().Name) == false).ToList();
                    }
                    else {
                        s_list = list;
                    }
                }
            }
        }

        return s_list;
    }
}
