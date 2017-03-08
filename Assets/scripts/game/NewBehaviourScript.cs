using UnityEngine;
using System.Collections;
using gameObjectFactory;
using System.Collections.Generic;
using superRaycast;
using superFunction;
using System;

public class NewBehaviourScript : MonoBehaviour {

	[SerializeField]
	private Transform unitContainer;

	[SerializeField]
	private GameObject quad;

	[SerializeField]
	private Camera battleCamera;

	private Battle battle;

	private Dictionary<int, GameObject> goDic = new Dictionary<int, GameObject>();

	private IUnitSDS getUnitSDS(int _id){

		return StaticData.GetData<UnitSDS>(_id);
	}

	void Awake(){

		Connection.Instance.Init (ConfigDictionary.Instance.ip, ConfigDictionary.Instance.port, GetBytes, ConfigDictionary.Instance.uid);

		battle = new Battle ();

		battle.Init (GameConfig.Instance, getUnitSDS);

		SuperRaycast.SetIsOpen (true, "1");

		SuperRaycast.SetCamera (battleCamera);

		SuperFunction.Instance.AddEventListener (quad, SuperRaycast.GetMouseButton, Hit);
	}

	private void GetBytes(byte[] _bytes){


	}

	private void Hit(int _index,object[] _datas){

		RaycastHit hit = (RaycastHit)_datas [0];

		Dictionary<int,Unit>.ValueCollection.Enumerator enumerator = battle.unitDic.Values.GetEnumerator ();

		while (enumerator.MoveNext ()) {

			enumerator.Current.SetTargetPos (new RVO.Vector2 (hit.point.x, hit.point.z));
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		if (Input.GetKeyUp (KeyCode.A)) {

			if (UnityEngine.Random.value < 0.5f) {

				battle.AddUnitToPool (true, 1);

			} else {

				battle.AddUnitToPool (true, 2);
			}
		}

		if (Input.GetKeyUp (KeyCode.B)) {

			if (UnityEngine.Random.value < 0.5f) {

				battle.AddUnitToPool (false, 1);

			} else {

				battle.AddUnitToPool (false, 2);
			}
		}

		if (Input.GetKeyUp (KeyCode.C)) {

			battle.Spawn ();
		}

		Refresh ();
	}

	void FixedUpdate(){

		battle.Update ();
	}

	private void Refresh(){
		
		Dictionary<int, Unit>.Enumerator enumerator = battle.unitDic.GetEnumerator ();

		while (enumerator.MoveNext ()) {

			int uid = enumerator.Current.Key;

			Unit unit = enumerator.Current.Value;

			if (goDic.ContainsKey (uid)) {

				GameObject go = goDic [uid];

				go.transform.localPosition = new Vector3 ((float)unit.pos.x, 0f, (float)unit.pos.y);

			} else {

				CreateGo (unit);
			}
		}
	}

	private void CreateGo(Unit _unit){

		Action<GameObject,string> cb = delegate(GameObject _go, string arg2) {

			if(_unit.isMine){

				_go.GetComponent<Renderer> ().material.SetColor ("_Color", Color.blue);

			}else{

				_go.GetComponent<Renderer> ().material.SetColor ("_Color", Color.yellow);
			}

			float scale = (float)_unit.sds.GetRadius () * 2;

			_go.transform.SetParent (unitContainer, false);

			_go.transform.localScale = new Vector3 (scale, scale, scale);

			_go.transform.localPosition = new Vector3 ((float)_unit.pos.x, 0f, (float)_unit.pos.y);

			goDic [_unit.uid] = _go;
		};

		GameObjectFactory.Instance.GetGameObject ("Assets/arts/prefab/hero.prefab", cb);
	}

	private void GetGo(GameObject _go,string _str){

//		_go.GetComponent<Renderer> ().material.SetColor ("_Color", Color.blue);
//
//		RVO.Vector2 pos = new RVO.Vector2 (Random.value * GameConfig.Instance.mapWidth + GameConfig.Instance.mapX, Random.value * GameConfig.Instance.mapHeight + GameConfig.Instance.mapY);
//
//		Unit unit = battle.AddUnit (1, true, 1, pos);
//
//		unitDic [unit.uid] = unit;
//
//		float scale = (float)unit.sds.GetRadius () * 2;
//
//		_go.transform.SetParent (unitContainer, false);
//
//		_go.transform.localScale = new Vector3 (scale, scale, scale);
//
//		_go.transform.localPosition = new Vector3 ((float)pos.x, 0f, (float)pos.y);
//
//		goDic [unit.uid] = _go;
	}


}
