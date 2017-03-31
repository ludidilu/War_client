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

	private const float AIR_UNIT_Y = 15f;

	[SerializeField]
	private Transform unitContainer;

	[SerializeField]
	private GameObject quad;

	[SerializeField]
	private Camera battleCamera;

	private Battle battle;

	private Dictionary<int, GameObject> unitGoDic = new Dictionary<int, GameObject>();

	private LinkedList<int> unitGoList = new LinkedList<int>();

	private Dictionary<int, GameObject> skillGoDic = new Dictionary<int, GameObject>();

	private LinkedList<int> skillGoList = new LinkedList<int>();

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

				HeroCellData cellData = new HeroCellData (sds.ID, false, 0, true);

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

		if (heroDic [_id].added) {

			UnitSDS sds = StaticData.GetData<UnitSDS> (_id);

			if (sds.skill != 0) {

				battle.ClientSendSkillCommand (_id, _x, _y);
			}

		} else {

			int money = battle.clientIsMine ? battle.mMoney : battle.oMoney;

			UnitSDS sds = StaticData.GetData<UnitSDS> (_id);

			if (money >= sds.prize) {

				battle.ClientSendHeroCommand (_id, _x, _y);
			}
		}
	}

	private void QuadClick(int _index, object[] _objs){

		if (heroSuperList.GetSelectedIndex () != -1) {

			RaycastHit hit = (RaycastHit)_objs [0];

			int fix = battle.clientIsMine ? 1 : -1;

			HeroCellClick (heroList [heroSuperList.GetSelectedIndex ()].id, hit.point.x * fix, hit.point.z * fix);

			heroSuperList.SetSelectedIndex (-1);
		}
	}

	public void BattleStart(){

		gameObject.SetActive (true);
	}

	private void BattleOver(){

		Dictionary<int,GameObject>.ValueCollection.Enumerator enumerator = unitGoDic.Values.GetEnumerator ();

		while (enumerator.MoveNext ()) {

			GameObject.Destroy (enumerator.Current);
		}

		enumerator = skillGoDic.Values.GetEnumerator ();

		while (enumerator.MoveNext ()) {

			GameObject.Destroy (enumerator.Current);
		}

		unitGoList.Clear ();

		unitGoDic.Clear ();

		skillGoList.Clear ();

		skillGoDic.Clear ();

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

		if (Input.GetKeyUp (KeyCode.F5)) {

			battle.ClientRequestRefresh ();
		}
	}

	void FixedUpdate(){

		battle.Update ();
	}

	private void Refresh(){

		RefreshUnit ();

		RefreshSkill ();

		RefreshUi ();
	}

	private void RefreshUnit(){

		int fix = battle.clientIsMine ? 1 : -1;

		Dictionary<int, Unit>.Enumerator enumerator = battle.unitDic.GetEnumerator ();

		while (enumerator.MoveNext ()) {

			int uid = enumerator.Current.Key;

			Unit unit = enumerator.Current.Value;

			if (unitGoDic.ContainsKey (uid)) {

				GameObject go = unitGoDic [uid];

				go.transform.localPosition = new Vector3 ((float)unit.pos.x * fix, go.transform.localPosition.y, (float)unit.pos.y * fix);

			} else {

				CreateUnitGo (unit);
			}
		}

		LinkedListNode<int> node = unitGoList.First;

		while (node != null) {

			LinkedListNode<int> nextNode = node.Next;

			int uid = node.Value;

			if (!battle.unitDic.ContainsKey (uid)) {

				GameObject go = unitGoDic [uid];

				GameObject.Destroy (go);

				unitGoDic.Remove (uid);

				unitGoList.Remove (node);
			}

			node = nextNode;
		}
	}

	private void RefreshSkill(){

		int fix = battle.clientIsMine ? 1 : -1;

		LinkedList<Skill>.Enumerator enumerator = battle.skillList.GetEnumerator ();

		while (enumerator.MoveNext ()) {

			Skill skill = enumerator.Current;

			if (skill.sds.GetEffectRadius () > 0) {

				if (skillGoDic.ContainsKey (skill.uid)) {

					GameObject go = skillGoDic [skill.uid];

					go.transform.localPosition = new Vector3 ((float)skill.pos.x * fix, go.transform.localPosition.y, (float)skill.pos.y * fix);

				} else {

					CreateSkillGo (skill);
				}
			}
		}

		LinkedListNode<int> node = skillGoList.First;

		while (node != null) {

			LinkedListNode<int> nextNode = node.Next;

			int uid = node.Value;

			enumerator = battle.skillList.GetEnumerator ();

			bool getSkill = false;

			while (enumerator.MoveNext ()) {

				if (enumerator.Current.uid == uid) {

					getSkill = true;

					break;
				}
			}

			if (!getSkill) {

				GameObject go = skillGoDic [uid];

				GameObject.Destroy (go);

				skillGoDic.Remove (uid);

				skillGoList.Remove (node);
			}

			node = nextNode;
		}
	}

	private void CreateUnitGo(Unit _unit){

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

			float y = _unit.sds.GetIsAirUnit() ? AIR_UNIT_Y : 0f;

			_go.transform.localPosition = new Vector3 ((float)_unit.pos.x * fix, y, (float)_unit.pos.y * fix);

			unitGoDic.Add(_unit.uid, _go);

			unitGoList.AddLast(_unit.uid);
		};

		GameObjectFactory.Instance.GetGameObject ("Assets/arts/prefab/hero.prefab", cb);
	}

	private void CreateSkillGo(Skill _skill){

		int fix = battle.clientIsMine ? 1 : -1;

		Action<GameObject,string> cb = delegate(GameObject _go, string arg2) {

			if(_skill.unit.isMine == battle.clientIsMine){

				_go.GetComponent<Renderer> ().material.SetColor ("_Color", Color.blue);

			}else{

				_go.GetComponent<Renderer> ().material.SetColor ("_Color", Color.yellow);
			}

			float scale = (float)_skill.sds.GetEffectRadius () * 2;

			_go.transform.SetParent (unitContainer, false);

			_go.transform.localScale = new Vector3 (scale, 0.1f, scale);

			_go.transform.localPosition = new Vector3 ((float)_skill.pos.x * fix, 0, (float)_skill.pos.y * fix);

			skillGoDic.Add(_skill.uid, _go);

			skillGoList.AddLast(_skill.uid);
		};

		GameObjectFactory.Instance.GetGameObject ("Assets/arts/prefab/hero.prefab", cb);
	}

	private void RefreshUi(){

		bool refresh = false;

		Dictionary<int, int> unitPool;

		Dictionary<int, Unit> heroPool;

		Dictionary<int, Dictionary<int, UnitCommandData>> unitCommandPool;

		Dictionary<int, HeroCommandData> heroCommandPool;

		Dictionary<int, SkillCommandData> skillCommandPool;

		if (battle.clientIsMine) {

			unitPool = battle.mUnitPool;

			heroPool = battle.mHeroPool;

			unitCommandPool = battle.mUnitCommandPool;

			heroCommandPool = battle.mHeroCommandPool;

			skillCommandPool = battle.mSkillCommandPool;

		} else {

			unitPool = battle.oUnitPool;

			heroPool = battle.oHeroPool;

			unitCommandPool = battle.oUnitCommandPool;

			heroCommandPool = battle.oHeroCommandPool;

			skillCommandPool = battle.oSkillCommandPool;
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

			bool added = heroPool.ContainsKey (cellData.id);

			int cd = 0;

			bool selectable = true;

			if (!added) {

				if (heroCommandPool.ContainsKey (cellData.id)) {

					added = true;

					selectable = false;
				}

			}else{

				Unit hero = heroPool[cellData.id];

				if(hero.sds.GetSkill() != 0){

					cd = hero.skillCd;
				}
			}

			if(selectable && added){

				if(cd > 0){

					selectable = false;

				}else{

					if(skillCommandPool.ContainsKey(cellData.id)){

						selectable = false;
					}
				}
			}

			if (cellData.added != added) {

				cellData.added = added;

				refresh = true;
			}

			if(cellData.cd != cd){

				cellData.cd = cd;

				refresh = true;
			}

			if(cellData.selectable != selectable){

				cellData.selectable = selectable;

				refresh = true;
			}
		}

		if (refresh) {

			heroSuperList.SetDataAndKeepLocation (heroList);
		}

		moneyTf.text = battle.clientIsMine ? battle.mMoney.ToString () : battle.oMoney.ToString ();
	}
}
