using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClownFish.Base.Json;
using ClownFish.Base.TypeExtend;
using Newtonsoft.Json;

namespace ClownFish.Base
{
	/// <summary>
	/// JSON序列化的工具类
	/// </summary>
	public static class JsonExtensions
	{
		/// <summary>
		/// 将一个对象序列化为JSON字符串。
		/// </summary>
		/// <param name="obj">要序列化的对象</param>
		/// <returns>序列化得到的JSON字符串</returns>
		public static string ToJson(this object obj)
		{
			DefaultJsonSerializer serializer = ObjectFactory.New<DefaultJsonSerializer>();
			return serializer.Serialize(obj, false);
		}

		/// <summary>
		/// 将一个对象序列化为JSON字符串。
		/// </summary>
		/// <param name="obj">要序列化的对象</param>
		/// <param name="keepType">尽量在序列化过程中保留类型信息（Newtonsoft.Json可支持）</param>
		/// <returns>序列化得到的JSON字符串</returns>
		public static string ToJson(this object obj, bool keepType)
		{
			DefaultJsonSerializer serializer = ObjectFactory.New<DefaultJsonSerializer>();
			return serializer.Serialize(obj, keepType);
		}


		/// <summary>
		/// 将一个JSON字符串反序列化为对象
		/// </summary>
		/// <typeparam name="T">反序列的对象类型参数</typeparam>
		/// <param name="json">JSON字符串</param>
		/// <returns>反序列化得到的结果</returns>
		public static T FromJson<T>(this string json)
		{
			DefaultJsonSerializer serializer = ObjectFactory.New<DefaultJsonSerializer>();
			return serializer.Deserialize<T>(json);
		}


		/// <summary>
		/// 将一个JSON字符串反序列化为对象
		/// </summary>
		/// <param name="json">JSON字符串</param>
		/// <param name="destType">反序列的对象类型参数</param>
		/// <returns>反序列化得到的结果</returns>
		public static object FromJson(this string json, Type destType)
		{
			DefaultJsonSerializer serializer = ObjectFactory.New<DefaultJsonSerializer>();
			return serializer.Deserialize(json, destType);
		}


        /// <summary>
        /// 将对象序列化成缩进结构的JSON，便于日志记录
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToDumpJson(this object obj)
        {
            if( obj == null )
                return "null";

            DefaultJsonSerializer serializer = ObjectFactory.New<DefaultJsonSerializer>();
            JsonSerializerSettings settings = serializer.GetJsonSerializerSettings(true);
            // 设置为缩进格式
            settings.Formatting = Formatting.Indented;
            return serializer.Serialize(obj, settings);
        }

	}
}
