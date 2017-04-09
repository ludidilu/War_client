using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.IO;
using superList;
using System.Collections.Generic;

public class Entrance : MonoBehaviour {

	[SerializeField]
	private SuperList superList;

	[SerializeField]
	private string[] scenes;

	private IUnitSDS getUnitSDS(int _id){

		return StaticData.GetData<UnitSDS>(_id);
	}

	private ISkillSDS getSkillSDS(int _id){

		return StaticData.GetData<SkillSDS> (_id);
	}

	private static void PrintLog(string _str)
	{
		SuperDebug.Log(_str);
	}

	private static void WriteLog(string _str){

		using (FileStream fs = new FileStream(ConfigDictionary.Instance.logPath, FileMode.Append))
		{
			using (StreamWriter bw = new StreamWriter(fs))
			{
				bw.Write(_str);

				fs.Flush();
			}
		}
	}

	// Use this for initialization
	void Awake () {

		ConfigDictionary.Instance.LoadLocalConfig (Application.streamingAssetsPath + "/local.xml");

		if (string.IsNullOrEmpty (ConfigDictionary.Instance.logPath)) {

			Log.Init (PrintLog, null);

		} else {

			FileInfo fi = new FileInfo (ConfigDictionary.Instance.logPath);

			if (fi.Exists) {

				fi.Delete ();
			}

			Log.Init (PrintLog, WriteLog);
		}

		GameConfig.Instance.LoadLocalConfig (ConfigDictionary.Instance.configPath);
		
		StaticData.path = ConfigDictionary.Instance.tablePath;
		
		StaticData.Dispose ();
		
		StaticData.Load<UnitSDS> ("unit");

		StaticData.Load<SkillSDS> ("skill");

		Battle.Init (GameConfig.Instance, getUnitSDS, getSkillSDS);

		Time.fixedDeltaTime = (float)GameConfig.Instance.timeStep;

		superList.CellClickHandle = Click;

		List<string> list = new List<string> (scenes);

		superList.SetData (list);
	}

	private void Click(object _obj){

		SceneManager.LoadScene ((string)_obj);
	}

	// Update is called once per frame
	void Update () {
	
	}
}
