namespace ClownFish.Base;

/// <summary>
/// 
/// </summary>
public static class AppPartUtils
{
    private static List<Assembly> s_list;
    private static readonly object s_syncLock = new object();

    /// <summary>
    /// 
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

                    s_list = list;
                }
            }
        }

        return s_list;
    }
}
