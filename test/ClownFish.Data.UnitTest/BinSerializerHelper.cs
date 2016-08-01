using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ClownFish.Data.UnitTest
{
	internal static class BinSerializerHelper
	{
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


		public static T Deserialize<T>(byte[] buffer)
		{
			return (T)DeserializeObject(buffer);
		}

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
	}
}
