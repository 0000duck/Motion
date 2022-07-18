using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace DMSkin.Sockets
{
	public class SerHelper
	{
		public static byte[] Serialize(object obj)
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			using (MemoryStream memoryStream = new MemoryStream())
			{
				binaryFormatter.Serialize(memoryStream, obj);
				return memoryStream.ToArray();
			}
		}

		public static T Deserialize<T>(byte[] buffer)
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			using (MemoryStream serializationStream = new MemoryStream(buffer))
			{
				return (T)binaryFormatter.Deserialize(serializationStream);
			}
		}

		public static object Deserialize(byte[] datas, int index)
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			MemoryStream memoryStream = new MemoryStream(datas, index, datas.Length - index);
			object result = binaryFormatter.Deserialize(memoryStream);
			memoryStream.Dispose();
			return result;
		}
	}
}
