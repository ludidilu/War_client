using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Entrance : MonoBehaviour {

	// Use this for initialization
	void Awake () {
	
		ConfigDictionary.Instance.LoadLocalConfig (Application.streamingAssetsPath + "/local.xml");

		GameConfig.Instance.LoadLocalConfig (ConfigDictionary.Instance.configPath);
		
		StaticData.path = ConfigDictionary.Instance.tablePath;
		
		StaticData.Dispose ();
		
		StaticData.Load<UnitSDS> ("unit");

		Time.fixedDeltaTime = (float)GameConfig.Instance.timeStep;

		SceneManager.LoadScene ("battle");

		Connection.Instance.Init (ConfigDictionary.Instance.ip, ConfigDictionary.Instance.port, GetBytes, ConfigDictionary.Instance.uid);
	}

	private void GetBytes(byte[] _bytes){


	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
