using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ClownFish.Base.Files;
using ClownFish.Base.Xml;

namespace ClownFish.Base.UnitTest._Sample
{
    class FileDependencyManager_DEMO
    {

        // FileDependencyManager 是什么？
        // =================================================

        // FileDependencyManager 封装了ASP.NET CACHE的文件缓存依赖功能，可用于从配置文件中获取参数，
        // 并且当配置文件发生修改后，能获取到最新的结果。


        // 使用FileDependencyManager时，需要将实例定义成静态字段，
        // 并为 FileDependencyManager 的构造方法提供二个参数：
        //  1、如何读取文件并生成实体对象，【建议】采用反序列化
        //  2、要监控哪些文件。当这些文件发生修改后，变量的值会自动更新。

        private static FileDependencyManager<List<User>>
            s_cacheItem = new FileDependencyManager<List<User>>(
                    files => XmlHelper.XmlDeserializeFromFile<List<User>>(files[0]),
                    Path.Combine(HttpRuntime.AppDomainAppPath, @"App_Data\Users.config"));


        // 下面的方法用于获取从配置读取到的结果

        public static List<User> Users {
            get { return s_cacheItem.Result; }
        }


        // 以上就是正确的使用方式
        // 1、定义一个静态字段，FileDependencyManager的实例
        // 2、用于获取配置文件的只读属性，基本上是固定写法，唯独差别就是泛型参数



























        public class User
        {
            // 一个实体类型，这里只是一个演示用的空类型
        }
    }
}
