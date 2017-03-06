using System.Collections;

public class ConfigDictionary : Config {

	private static ConfigDictionary _Instance;

	public static ConfigDictionary Instance{

		get{

			if (_Instance == null) {

				_Instance = new ConfigDictionary ();
			}

			return _Instance;
		}
	}

	public string tablePath;
	public string configPath;
}
