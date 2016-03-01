using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClownFish.Base.Framework;
using Newtonsoft.Json;

namespace ClownFish.Base.Json
{


//警告：如果需要修改JSON序列化方式，需要完成2件事件：
//    1. 重新实现JsonSerializer定义的虚方法
//    2. 重新实现ActionParametersProviderFactory


	/// <summary>
	/// 用于设置默认的JSON序列化配置事件的委托定义
	/// </summary>
	/// <param name="settings">JsonSerializerSettings实例引用，用于在事件处理器上修改属性，改变序列化行为</param>
	/// <param name="isSerialize">当时事件触发是不是发生在序列化过程中，true: 是， false： 否</param>
	public delegate void SetDefaultJsonSerializerSettingsEventHandler(JsonSerializerSettings settings, bool isSerialize);


	/// <summary>
	/// Json序列化的默认封装，不能建议直接使用，提供这个类型主要是为了可以替换JSON序列化的实现。
	/// 推荐使用JsonExtensions提供的方法。
	/// </summary>
	public class DefaultJsonSerializer
	{
		/// <summary>
		/// 设置默认的JSON序列化配置事件
		/// </summary>
		public static event SetDefaultJsonSerializerSettingsEventHandler SetDefaultJsonSerializerSettings;


		/// <summary>
		/// 获取默认的序列化设置
		/// </summary>
		/// <returns></returns>
		public virtual JsonSerializerSettings GetJsonSerializerSettings(bool isSerialize)
		{
			JsonSerializerSettings settings = JsonConvert.DefaultSettings == null 
							? new JsonSerializerSettings()
							: JsonConvert.DefaultSettings();

			settings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
            settings.Converters.Add(XmlCdataJsonConverter.Instance);

			SetDefaultJsonSerializerSettingsEventHandler eventHandler = SetDefaultJsonSerializerSettings;
			if( eventHandler != null )
				eventHandler(settings, isSerialize);

			return settings;
		}


		/// <summary>
		/// 将一个对象序列化为JSON字符串。
		/// </summary>
		/// <param name="obj">要序列化的对象</param>
		/// <param name="keepType">尽量在序列化过程中保留类型信息（Newtonsoft.Json可支持）</param>
		/// <returns>序列化得到的JSON字符串</returns>
		public virtual string Serialize(object obj, bool keepType)
		{
			// 扩展点：允许重新指定JSON序列化实现方式

			if( obj == null )
				throw new ArgumentNullException("obj");

			JsonSerializerSettings settings = GetJsonSerializerSettings(true);

			if( keepType )
				settings.TypeNameHandling = TypeNameHandling.All;

			if( WebConfig.IsDebugMode )
				settings.Formatting = Formatting.Indented;

			return Serialize(obj, settings);
		}


		/// <summary>
		/// 将一个对象序列化为JSON字符串。
		/// </summary>
		/// <param name="obj">要序列化的对象</param>
		/// <param name="settings">序列化参数</param>
		/// <returns></returns>
		public virtual string Serialize(object obj, JsonSerializerSettings settings)
		{
			if( obj == null )
				throw new ArgumentNullException("obj");

			if( settings == null ) 
				return JsonConvert.SerializeObject(obj);

			else
				return JsonConvert.SerializeObject(obj, settings);
		}


		/// <summary>
		/// JSON反序列化
		/// </summary>
		/// <typeparam name="T">期望反序列化得到的对象类型</typeparam>
		/// <param name="json">JSON字符串</param>
		/// <returns></returns>
		public virtual  T Deserialize<T>(string json)
		{
			if( string.IsNullOrEmpty(json) )
				throw new ArgumentNullException("json");

			JsonSerializerSettings settings = GetJsonSerializerSettings(false);

			return JsonConvert.DeserializeObject<T>(json, settings);
		}

		/// <summary>
		/// JSON反序列化
		/// </summary>
		/// <param name="json">JSON字符串</param>
		/// <param name="destType">期望反序列化得到的对象类型</param>
		/// <returns></returns>
		public virtual object Deserialize(string json, Type destType)
		{
			if( string.IsNullOrEmpty(json) )
				throw new ArgumentNullException("json");
			if( destType == null )
				throw new ArgumentNullException("destType");

			JsonSerializerSettings settings = GetJsonSerializerSettings(false);

			return JsonConvert.DeserializeObject(json, destType, settings);
		}



	}
}
