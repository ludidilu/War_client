using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Entrance : MonoBehaviour {

	private IUnitSDS getUnitSDS(int _id){

		return StaticData.GetData<UnitSDS>(_id);
	}

	private static void WriteLog(string _str)
	{
		SuperDebug.Log(_str);
	}

	// Use this for initialization
	void Awake () {
	
		Log.Init (WriteLog);

		ConfigDictionary.Instance.LoadLocalConfig (Application.streamingAssetsPath + "/local.xml");

		GameConfig.Instance.LoadLocalConfig (ConfigDictionary.Instance.configPath);
		
		StaticData.path = ConfigDictionary.Instance.tablePath;
		
		StaticData.Dispose ();
		
		StaticData.Load<UnitSDS> ("unit");

		Battle.Init (GameConfig.Instance, getUnitSDS);

		Time.fixedDeltaTime = (float)GameConfig.Instance.timeStep;

		SceneManager.LoadScene ("battle");
	}

	private void GetBytes(byte[] _bytes){


	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
