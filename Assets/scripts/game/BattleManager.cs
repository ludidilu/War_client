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

		SuperRaycast.checkBlockByUi = true;

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

		SuperFunction.Instance.AddEventListener (quad, SuperRaycast.GetMouseClick, QuadClick);
	}

	private void UnitCellClick(object _data){

		UnitCellData data = _data as UnitCellData;

		int money = battle.clientIsMine ? battle.mMoney : battle.oMoney;

		UnitSDS sds = StaticData.GetData<UnitSDS> (data.id);

		if (money >= sds.prize) {

			battle.ClientSendUnitCommand (data.id);
		}
	}

	private void HeroCellClick(int _id, float _x, float _y){

		int money = battle.clientIsMine ? battle.mMoney : battle.oMoney;

		UnitSDS sds = StaticData.GetData<UnitSDS> (_id);

		if (money >= sds.prize) {

			battle.ClientSendHeroCommand (_id, _x, _y);
		}
	}

	private void QuadClick(int _index, object[] _objs){

		if (heroSuperList.GetSelectedIndex () != -1) {

			RaycastHit hit = (RaycastHit)_objs [0];

			int fix = battle.clientIsMine ? 1 : -1;

			HeroCellClick (heroList [heroSuperList.GetSelectedIndex ()].id, hit.point.x * fix, hit.point.z * fix);
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

		Dictionary<int, int> unitPool;

		Dictionary<int, Unit> heroPool;

		Dictionary<int, Dictionary<int, UnitCommandData>> unitCommandPool;

		Dictionary<int, HeroCommandData> heroCommandPool;

		if (battle.clientIsMine) {

			unitPool = battle.mUnitPool;

			heroPool = battle.mHeroPool;

			unitCommandPool = battle.mUnitCommandPool;

			heroCommandPool = battle.mHeroCommandPool;

		} else {

			unitPool = battle.oUnitPool;

			heroPool = battle.oHeroPool;

			unitCommandPool = battle.oUnitCommandPool;

			heroCommandPool = battle.oHeroCommandPool;
		}

		Dictionary<int, UnitCellData>.ValueCollection.Enumerator enumerator = unitDic.Values.GetEnumerator ();

		while (enumerator.MoveNext ()) {

			UnitCellData cellData = enumerator.Current;

			int num;

			if (unitPool.ContainsKey (cellData.id)) {

				num = unitPool [cellData.id];

			} else {

				num = 0;
			}

			Dictionary<int, Dictionary<int, UnitCommandData>>.ValueCollection.Enumerator enumerator3 = unitCommandPool.Values.GetEnumerator ();

			while (enumerator3.MoveNext ()) {

				Dictionary<int, UnitCommandData>.ValueCollection.Enumerator enumerator4 = enumerator3.Current.Values.GetEnumerator ();

				while (enumerator4.MoveNext ()) {

					if (enumerator4.Current.id == cellData.id) {

						num++;
					}
				}
			}

			if (cellData.num != num) {

				cellData.num = num;

				refresh = true;
			}
		}

		if (refresh) {

			unitSuperList.SetDataAndKeepLocation (unitList);

			refresh = false;
		}

		Dictionary<int, HeroCellData>.ValueCollection.Enumerator enumerator2 = heroDic.Values.GetEnumerator ();

		while (enumerator2.MoveNext ()) {

			HeroCellData cellData = enumerator2.Current;

			bool b = heroPool.ContainsKey (cellData.id);

			if (!b) {

				if (heroCommandPool.ContainsKey (cellData.id)) {

					b = true;
				}
			}

			if (cellData.added != b) {

				cellData.added = b;

				refresh = true;
			}
		}

		if (refresh) {

			int lastIndex = heroSuperList.GetSelectedIndex ();

			heroSuperList.SetDataAndKeepLocation (heroList);

			if (lastIndex != -1) {

				if (heroList [lastIndex].added) {

					heroSuperList.SetSelectedIndex (-1);
				}
			}
		}

		moneyTf.text = battle.clientIsMine ? battle.mMoney.ToString () : battle.oMoney.ToString ();

//		if (heroSuperList.GetSelectedIndex () != -1) {
//
//			if (Input.GetMouseButtonUp(0)) {
//
//				Ray ray = battleCamera.ScreenPointToRay(Input.mousePosition);
//
//				RaycastHit hit;
//
//				bool b = Physics.Raycast (ray, out hit);
//
//				if (b) {
//
//					int fix = battle.clientIsMine ? 1 : -1;
//
//					HeroCellClick (heroList[heroSuperList.GetSelectedIndex ()].id, hit.point.x * fix, hit.point.z * fix);
//				}
//			}
//		}

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
