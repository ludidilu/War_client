using UnityEngine;
using System.Collections;
using gameObjectFactory;
using System.Collections.Generic;
using superRaycast;
using superFunction;
using System;
using System.IO;
using superList;
using UnityEngine.UI;

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

	//----ui
	[SerializeField]
	private SuperList unitSuperList;

	[SerializeField]
	private SuperList heroSuperList;

	[SerializeField]
	private Text moneyTf;

	private List<UnitCellData> unitList = new List<UnitCellData> ();

	private Dictionary<int, UnitCellData> unitDic = new Dictionary<int, UnitCellData> ();

	private List<HeroCellData> heroList = new List<HeroCellData> ();

	private Dictionary<int, HeroCellData> heroDic = new Dictionary<int, HeroCellData>();
	//----

	public void Init(Action<MemoryStream> _sendDataCallBack, Action _battleOverCallBack){

		battle = new Battle ();

		battle.ClientInit (_sendDataCallBack, Refresh, BattleOver, SendCommandOK);

		SuperRaycast.SetIsOpen (true, "1");

		SuperRaycast.SetCamera (battleCamera);

		battleOverCallBack = _battleOverCallBack;

		Dictionary<int, UnitSDS> tmpDic = StaticData.GetDic<UnitSDS> ();

		Dictionary<int, UnitSDS>.ValueCollection.Enumerator enumerator = tmpDic.Values.GetEnumerator ();

		while (enumerator.MoveNext ()) {

			UnitSDS sds = enumerator.Current;

			if (sds.isHero) {

				HeroCellData cellData = new HeroCellData (sds.ID, false);

				heroList.Add (cellData);

				heroDic.Add (sds.ID, cellData);

			} else {

				UnitCellData cellData = new UnitCellData (sds.ID, 0);

				unitList.Add (cellData);

				unitDic.Add (sds.ID, cellData);
			}
		}

		unitSuperList.SetData (unitList);

		heroSuperList.SetData (heroList);

		unitSuperList.CellClickHandle = UnitCellClick;

		gameObject.SetActive (false);
	}

	private void UnitCellClick(object _data){

		UnitCellData data = _data as UnitCellData;

		int money = battle.clientIsMine ? battle.mMoney : battle.oMoney;

		UnitSDS sds = StaticData.GetData<UnitSDS> (data.id);

		if (money >= sds.prize) {

			battle.ClientSendUnitCommand (data.id);
		}
	}

	public void BattleStart(){

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

	private void SendCommandOK(bool _b){


	}

	public void GetBytes(byte[] _bytes){

		battle.ClientGetBytes (_bytes);
	}
	
	// Update is called once per frame
	void Update () {

		bool refresh = false;

		Dictionary<int, int> unitPool = battle.clientIsMine ? battle.mUnitPool : battle.oUnitPool;

		Dictionary<int, int>.Enumerator enumerator = unitPool.GetEnumerator ();

		while (enumerator.MoveNext ()) {

			UnitCellData cellData = unitDic [enumerator.Current.Key];

			if (cellData.num != enumerator.Current.Value) {

				cellData.num = enumerator.Current.Value;

				refresh = true;
			}
		}

		if (refresh) {

			unitSuperList.SetDataAndKeepLocation (unitList);

			refresh = false;
		}

		Dictionary<int, Unit> heroPool = battle.clientIsMine ? battle.mHeroPool : battle.oHeroPool;

		Dictionary<int, HeroCellData>.Enumerator enumerator2 = heroDic.GetEnumerator ();

		while (enumerator2.MoveNext ()) {

			bool b = heroPool.ContainsKey (enumerator2.Current.Key);

			if (b != enumerator2.Current.Value.added) {

				enumerator2.Current.Value.added = b;

				refresh = true;
			}
		}

		if (refresh) {

			heroSuperList.SetDataAndKeepLocation (heroList);
		}

		moneyTf.text = battle.clientIsMine ? battle.mMoney.ToString () : battle.oMoney.ToString ();

		if (Input.GetKeyUp (KeyCode.F5)) {

			battle.ClientRequestRefresh ();
		}
	}

	void FixedUpdate(){

		battle.Update ();
	}

	private void Refresh(){

		int fix = battle.clientIsMine ? 1 : -1;
		
		Dictionary<int, Unit>.Enumerator enumerator = battle.unitDic.GetEnumerator ();

		while (enumerator.MoveNext ()) {

			int uid = enumerator.Current.Key;

			Unit unit = enumerator.Current.Value;

			if (goDic.ContainsKey (uid)) {

				GameObject go = goDic [uid];

				go.transform.localPosition = new Vector3 ((float)unit.pos.x * fix, 0f, (float)unit.pos.y * fix);

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

		int fix = battle.clientIsMine ? 1 : -1;

		Action<GameObject,string> cb = delegate(GameObject _go, string arg2) {

			if(_unit.isMine == battle.clientIsMine){

				_go.GetComponent<Renderer> ().material.SetColor ("_Color", Color.blue);

			}else{

				_go.GetComponent<Renderer> ().material.SetColor ("_Color", Color.yellow);
			}

			float scale = (float)_unit.sds.GetRadius () * 2;

			_go.transform.SetParent (unitContainer, false);

			_go.transform.localScale = new Vector3 (scale, scale, scale);

			_go.transform.localPosition = new Vector3 ((float)_unit.pos.x * fix, 0f, (float)_unit.pos.y * fix);

			goDic.Add(_unit.uid, _go);

			goList.AddLast(_unit.uid);
		};

		GameObjectFactory.Instance.GetGameObject ("Assets/arts/prefab/hero.prefab", cb);
	}
}
