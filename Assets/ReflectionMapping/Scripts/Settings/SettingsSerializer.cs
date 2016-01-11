using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;
using UnityEngine;

public class SettingsSerializer<T>
{

	private readonly string fileName;
	private string filePath {
		get { return Path.Combine (Application.persistentDataPath, fileName); }
	}
	
	public SettingsSerializer(string filename)
	{
		this.fileName = filename;
	}
	
	public void Save (T s)
	{
		XmlSerializer writer =
            new XmlSerializer (typeof(T));

		using (StreamWriter file = new StreamWriter(filePath)) {
			writer.Serialize (file, s);
		}
		
		Debug.Log ("Saved to: " + filePath);
	}

	public T Load ()
	{
		using (FileStream fs = new FileStream(filePath, FileMode.Open)) {
			XmlSerializer serializer = new XmlSerializer (typeof(T));
			T s = (T)serializer.Deserialize (fs);
			
			Debug.Log ("Loaded from: " + filePath);
			return s;
		}
	}
	
	public bool Delete ()
	{
		try { 
			File.Delete (filePath); 
			Debug.Log ("Deleted: " + filePath);
			return true;
		} catch {
			return false;
		}
	}
}
