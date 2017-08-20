using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Base
{
	/// <summary>
	/// 二进制序列化的工具类
	/// </summary>
	public static class BinSerializerHelper
	{
		/// <summary>
		/// 将对象序列化成二进制数组
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static byte[] Serialize(object obj)
		{
			if( obj == null )
				throw new ArgumentNullException(nameof(obj));

			using( MemoryStream stream = new MemoryStream() ) {
				BinaryFormatter formatter = new BinaryFormatter();
				formatter.Serialize(stream, obj);

				stream.Position = 0;
				return stream.ToArray();
			}
		}

		/// <summary>
		/// 从二进制数组中反序列化
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="buffer"></param>
		/// <returns></returns>
		public static T Deserialize<T>(byte[] buffer)
		{
			return (T)DeserializeObject(buffer);
		}

		/// <summary>
		/// 从二进制数组中反序列化
		/// </summary>
		/// <param name="buffer"></param>
		/// <returns></returns>
		public static object DeserializeObject(byte[] buffer)
		{
			if( buffer == null )
				throw new ArgumentNullException(nameof(buffer));

			using( MemoryStream stream = new MemoryStream(buffer) ) {
				stream.Position = 0;

				BinaryFormatter formatter = new BinaryFormatter();
				return formatter.Deserialize(stream);
			}
		}

		/// <summary>
		/// 采用二进制序列化的方式克隆对象
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static T CloneObject<T>(this T obj)
		{
			byte[] bb = Serialize(obj);
			return Deserialize<T>(bb);
		}
	}
}
