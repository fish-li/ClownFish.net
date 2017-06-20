using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Data
{
    /// <summary>
    /// 一些与命令相关的扩展工具类
    /// </summary>
    public static class CommandExtensions
    {
        /// <summary>
        /// 设置当前要执行的命令对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static T SetCommand<T>(this T command, Action<DbCommand> action) where T : BaseCommand
        {
            // 说明：其实没有这个方法也可以，只是写代码就不够顺畅了

            // 例如：CPQuery.Create(...).SetCommand(...).ExecuteNonQuery();
            // 对应写法：
            // CPQuery query = CPQuery.Create(...);
            // query.Command = ..........
            // query.ExecuteNonQuery();

            if( action != null )
                action.Invoke(command.Command);
            return command;
        }
    }
}
