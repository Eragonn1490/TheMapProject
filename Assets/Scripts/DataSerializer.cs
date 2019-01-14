using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class DataSerializer
{
	public static void Save<T>(T file, string name)
	{
		BinaryFormatter bf = new BinaryFormatter();
		FileStream fs = File.Create(name);
		bf.Serialize(fs, file);
		fs.Close();
	}

	public static T Load<T>(string name)
	{
		if (File.Exists(name))
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream fs = File.Open(name, FileMode.Open);
			T output = (T)bf.Deserialize(fs);
			fs.Close();
			return output;
		}
		else
		{
			return default(T);
		}
	}

	public static bool CheckExistence(string name)
	{
		return File.Exists(name);
	}
}