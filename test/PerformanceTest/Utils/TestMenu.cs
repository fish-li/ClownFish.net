namespace PerformanceTest.Utils;

public static class TestMenu
{
    public static async Task Run()
    {
        MenuInit();

        while( true ) {
            MenuMethodAttribute attr = ShowMenu();

            if( attr == null )
                break;

            await Execute(attr);
        }
    }



    private static List<MenuMethodAttribute> s_methods;

    private static void MenuInit()
    {
        s_methods = new List<MenuMethodAttribute>();

        var allMethod = (from t in Assembly.GetEntryAssembly().GetExportedTypes()
                         from m in t.GetMethods(BindingFlags.Static | BindingFlags.Public)
                         let a = m.GetMyAttribute<MenuMethodAttribute>()
                         where a != null
                         select m
                     ).ToList();


        
        foreach( var m in allMethod ) {

            MenuMethodAttribute attr = m.GetMyAttribute<MenuMethodAttribute>();
            attr.Method = m;

            if( attr.Group.IsNullOrEmpty() )
                attr.Group = m.DeclaringType.Name;

            s_methods.Add(attr);
        }

        // 一个组的测试连续在一起
        s_methods = s_methods.OrderBy(x => x.Group).ToList();

        // 产生序号
        int index = 0;
        foreach( var attr in s_methods ) {
            attr.Index = (++index).ToString();
        }
    }

    private static bool s_first = true;

    private static MenuMethodAttribute ShowMenu()
    {
        if( s_first == false ) {
            Console.WriteLine();
            Console.Write("Press any key to continue .......");
            Console.ReadKey();
            s_first = true;
        }

        while( true ) {               
            Console.Clear();

            string lastGroup = null;
            Console.WriteLine("ThreadPool.ThreadCount: " + System.Threading.ThreadPool.ThreadCount.ToString());
            Console.WriteLine("功能菜单： （X: Exit）");
            Console2.WriteSeparatedLine();

            foreach( var x in s_methods ) {

                if( lastGroup == null ) {
                    lastGroup = x.Group;
                }
                else {
                    if( lastGroup != x.Group ) {
                        Console2.WriteSeparatedLine();
                        lastGroup = x.Group;
                    }
                }

                Console.WriteLine($"{x.Index,3} : {x.Title}");
            }

            Console2.WriteSeparatedLine();
            Console.Write("请输入功能编号：");
            string input = Console.ReadLine();

            if( input.IsNullOrEmpty() )
                continue;

            if( input.Is("x") )
                return null;

            var attr = s_methods.FirstOrDefault(x => x.Index == input);
            if( attr != null )
                return attr;
        }
    }

    private static async Task Execute(MenuMethodAttribute attr)
    {
        try {
            if( attr.Method.ReturnType == typeof(Task) ) {
                Task task = (Task)attr.Method.Invoke(null, null);
                await task;
            }
            else {
                attr.Method.Invoke(null, null);
            }
        }
        catch( Exception ex ) {
            Console.WriteLine(ex.ToString2());
        }

        Console2.WriteSeparatedLine();
        Console.WriteLine("方法执行结束，按回车回到功能菜单……");
        Console.ReadLine();
    }
}
