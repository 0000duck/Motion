using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Tanac.Utils
{
    public class SerializeUtils
    {
		public static void BinarySerialize<T>(string path, List<T> list)
		{
			if (list != null)
			{
				using (FileStream fileStream = new FileStream(path, FileMode.Create))
				{
					BinaryFormatter b = new BinaryFormatter();
					b.Serialize(fileStream, list);
				}
			}
		}

		public static List<T> BinaryDeserialize<T>(string path)
		{
			if (!File.Exists(path))
			{
				return new List<T>();
			}
			using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				BinaryFormatter b = new BinaryFormatter();
				return (List<T>)b.Deserialize(fileStream);
			}
		}

		public static void JsonSerialize<T>(string path, List<T> list)
		{
			if (list != null)
			{
				string json = JsonConvert.SerializeObject(list);
				FileUtils.WriteText(path, json);
			}
		}

		public static List<T> JsonDeserialize<T>(string path)
		{
			if (!File.Exists(path))
			{
				return new List<T>();
			}
			string json = FileUtils.ReadAllText(path);
			return JsonConvert.DeserializeObject<List<T>>(json);
		}
	}
}
