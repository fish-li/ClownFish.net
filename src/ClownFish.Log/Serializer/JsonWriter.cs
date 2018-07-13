using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using ClownFish.Log.Configuration;

namespace ClownFish.Log.Serializer
{
	/// <summary>
	/// 将日志记录到JSON文件的写入器
	/// </summary>
	public class JsonWriter : FileWriter
    {

		#region ILogWriter 成员

        
        /// <summary>
        /// 根据指定的类型，获取对应的日志文件全路径
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public override string GetFilePath(Type t)
		{
			return string.Format(@"{0}{1}\{2}_{3}.json.log",
								s_rootDirectory, t.Name, t.Name, DateTime.Now.ToString("yyyy-MM-dd"));
		}

        /// <summary>
        /// 将对象转成要保存的文本
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected override string ObjectToText(object obj)
        {
            return ClownFish.Base.JsonExtensions.ToJson(obj);
        }



		#endregion
	}
}
