using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using ClownFish.Http.Pipleline;
using ClownFish.WebHost.Config;
using ClownFish.WebHost.Utils;

namespace ClownFish.WebApi.Controllers
{
    /// <summary>
    /// 用于创建StaticFileHandler的工厂类型
    /// </summary>
    internal class StaticFileHandlerFactory
    {
        public static readonly StaticFileHandlerFactory Instance = new StaticFileHandlerFactory();

        private Dictionary<string, int> _staticFileExtNames;
        private string _websitePath;


        internal void Init()
        {
            ServerOption option = ServerOption.Get();

            if( option.Website == null )
                return;

            if( option.Website.StaticFiles.IsNullOrEmpty() == false )
                _staticFileExtNames = (from x in option.Website.StaticFiles
                                      select x.Ext
                                    ).ToDictionary(x => x, x => 1, StringComparer.OrdinalIgnoreCase);


            _websitePath = option.Website.LocalPath; // ?? AppDomain.CurrentDomain.BaseDirectory;

            StaticFileHandler.Init(option);
        }


        /// <summary>
        /// 判断当前请求是不是静态文件，如果是则返回对应的Action对象
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public StaticFileHandler GetHandler(NHttpContext context)
        {
            if( _websitePath == null || _staticFileExtNames == null )
                return null;


            string path = context.Request.Path;

            // 先做 内存 判断
            string extName = PathHelper.GetExtension(path);
            if( _staticFileExtNames.ContainsKey(extName) == false )
                return null;

            // 再做 IO 判断
            string physicalPath = Path.Combine(_websitePath, path.TrimStart('/'));
            if( File.Exists(physicalPath) == false )
                return null;

            return StaticFileHandler.Create(physicalPath);
        }


        

    }
}
