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

                    s_list = GetApplicationPartAsmList0(list);
                }
            }
        }

        return s_list;
    }


    internal static List<Assembly> GetApplicationPartAsmList0(List<Assembly> list)
    {
        List<Assembly> result = null;

        // 允许/排除  一些应用程序模块
        // 例如：对于 AllInOne 的程序，在实际部署时可能需要 [禁用] 或者 [只启用] 其中的个别模块

        string allows = LocalSettings.GetSetting("ClownFish_EnableAppParts");
        if( allows.HasValue() ) {
            string[] names = allows.ToArray2();
            result = list.Where(x => names.Contains(x.GetName().Name)).ToList();
        }
        else {
            string ignores = LocalSettings.GetSetting("ClownFish_IgnoreAppParts");
            if( ignores.HasValue() ) {
                string[] names = ignores.ToArray2();
                result = list.Where(x => names.Contains(x.GetName().Name) == false).ToList();
            }
            else {
                result = list;
            }
        }

        // 无论如何，程序启动入口的程序集总是应该包含的
        Assembly exeAsm = AsmHelper.GetEntryAssembly();
        if( result.Contains(exeAsm) == false )
            result.Add(exeAsm);

        return result;
    }
}
