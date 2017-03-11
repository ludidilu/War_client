using UnityEngine;
using System.Collections;
using gameObjectFactory;
using System.Collections.Generic;
using superRaycast;
using superFunction;
using System;
using System.IO;

public class BattleManager : MonoBehaviour {

	[SerializeField]
	private Transform unitContainer;

	[SerializeField]
	private GameObject quad;

	[SerializeField]
	private Camera battleCamera;

	private Battle battle;

	private Dictionary<int, GameObject> goDic = new Dictionary<int, GameObject>();

	private LinkedList<int> goList = new LinkedList<int>();

	private Action battleOverCallBack;

	public void Init(Action<MemoryStream> _sendDataCallBack, Action _battleOverCallBack){

		battle = new Battle ();

		battle.ClientInit (_sendDataCallBack, Refresh, BattleOver);

		SuperRaycast.SetIsOpen (true, "1");

		SuperRaycast.SetCamera (battleCamera);

		battleOverCallBack = _battleOverCallBack;
	}

	public void BattleStart(){

		battle.ClientRequestRefresh ();

		gameObject.SetActive (true);
	}

	private void BattleOver(){

		Dictionary<int,GameObject>.ValueCollection.Enumerator enumerator = goDic.Values.GetEnumerator ();

		while (enumerator.MoveNext ()) {

			GameObject.Destroy (enumerator.Current);
		}

		goList.Clear ();

		goDic.Clear ();

		gameObject.SetActive (false);

		battleOverCallBack ();
	}

	public void GetBytes(byte[] _bytes){

		battle.ClientGetBytes (_bytes);
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyUp (KeyCode.Alpha1)) {

			battle.ClientSendCommand (1);
		}

		if (Input.GetKeyUp (KeyCode.Alpha2)) {

			battle.ClientSendCommand (2);
		}
//
//		if (Input.GetKeyUp (KeyCode.Alpha3)) {
//
//			battle.ClientSendCommand (false, 1);
//		}
//
//		if (Input.GetKeyUp (KeyCode.Alpha4)) {
//
//			battle.ClientSendCommand (false, 2);
//		}

		if (Input.GetKeyUp (KeyCode.F5)) {

			battle.ClientRequestRefresh ();
		}
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

		LinkedListNode<int> node = goList.First;

		while (node != null) {

			LinkedListNode<int> nextNode = node.Next;

			int uid = node.Value;

			if (!battle.unitDic.ContainsKey (uid)) {

				GameObject go = goDic [uid];

				GameObject.Destroy (go);

				goDic.Remove (uid);

				goList.Remove (node);
			}

			node = nextNode;
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

			goDic.Add(_unit.uid, _go);

			goList.AddLast(_unit.uid);
		};

		GameObjectFactory.Instance.GetGameObject ("Assets/arts/prefab/hero.prefab", cb);
	}
}
