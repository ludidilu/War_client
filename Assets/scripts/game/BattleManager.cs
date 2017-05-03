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
using publicTools;

public class BattleManager : MonoBehaviour {

	private const float AIR_UNIT_Y = 15f;

	[SerializeField]
	public Transform unitContainer;

	[SerializeField]
	private GameObject quad;

	[SerializeField]
	private Camera battleCamera;

	public Battle battle;

	public Dictionary<int, HeroStateMachine2> unitGoDic = new Dictionary<int, HeroStateMachine2>();

	private LinkedList<int> unitGoList = new LinkedList<int>();

	private Dictionary<int, GameObject> skillGoDic = new Dictionary<int, GameObject>();

	private LinkedList<int> skillGoList = new LinkedList<int>();

	private LinkedList<GameObject> skillPool = new LinkedList<GameObject>();

	private Action overCallBack;

	private bool isStart = false;

	//----ui
	[SerializeField]
	private SuperList unitSuperList;

	[SerializeField]
	private SuperList heroSuperList;

	[SerializeField]
	private Text moneyTf;

	[SerializeField]
	private GameObject alertPanel;

	[SerializeField]
	private Text alertText;

	private List<UnitCellData> unitList = new List<UnitCellData> ();

	private Dictionary<int, UnitCellData> unitDic = new Dictionary<int, UnitCellData> ();

	private List<HeroCellData> heroList = new List<HeroCellData> ();

	private Dictionary<int, HeroCellData> heroDic = new Dictionary<int, HeroCellData>();
	//----

	public void Init(Action<MemoryStream> _sendDataCallBack, Action _overCallBack){

		battle = new Battle ();

		overCallBack = _overCallBack;

		battle.ClientInit (_sendDataCallBack, Refresh, SendCommandOK, BattleOver);

		SuperRaycast.SetIsOpen (true, "1");

		SuperRaycast.checkBlockByUi = true;

		SuperRaycast.SetCamera (battleCamera);

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

		SuperFunction.Instance.AddEventListener<RaycastHit, int> (quad, SuperRaycast.GetMouseClick, QuadClick);
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

	private void QuadClick(int _index, RaycastHit hit, int _data){

		if (heroSuperList.GetSelectedIndex () != -1) {

			int fix = battle.clientIsMine ? 1 : -1;

			HeroCellClick (heroList [heroSuperList.GetSelectedIndex ()].id, hit.point.x * fix, hit.point.z * fix);

			heroSuperList.SetSelectedIndex (-1);
		}
	}

	public void BattleStart(){

		isStart = true;

		gameObject.SetActive (true);
	}

	private void BattleOver(bool _mWin, bool _oWin){

		isStart = false;

		alertPanel.SetActive (true);

		string str;

		if (_mWin && !_oWin) {

			if (battle.clientIsMine) {

				str = "You win!";

			} else {

				str = "You lose!";
			}

		} else if (!_mWin & _oWin) {

			if (!battle.clientIsMine) {

				str = "You win!";

			} else {

				str = "You lose!";
			}

		} else {

			str = "Draw game!";
		}

		alertText.text = str;
	}

	public void AlertPanelClick(){

		alertPanel.SetActive (false);

		BattleOverReal ();
	}

	private void BattleOverReal(){

		LinkedList<int>.Enumerator enumerator = unitGoList.GetEnumerator ();

		while (enumerator.MoveNext ()) {

			RemoveUnitGo (enumerator.Current);
		}

		enumerator = skillGoList.GetEnumerator ();

		while (enumerator.MoveNext ()) {

			RemoveSkillGo (enumerator.Current);
		}

		unitGoList.Clear ();

		skillGoList.Clear ();

		gameObject.SetActive (false);

		if (overCallBack != null) {

			overCallBack ();
		}
	}

	private void SendCommandOK(bool _b){


	}

	public void GetBytes(byte[] _bytes){

		battle.ClientGetBytes (_bytes);
	}
	
	// Update is called once per frame
	void Update () {

		if (isStart) {

			if (Input.GetKeyUp (KeyCode.F5)) {

				battle.ClientRequestRefresh ();
			}
		}
	}

	void FixedUpdate(){

		if (isStart) {

			bool mWin;
			bool oWin;

			battle.Update (out mWin, out oWin);
		}
	}

	private void Refresh(Dictionary<int,LinkedList<int>> _clientAttackData){

		RefreshUnit (_clientAttackData);

		RefreshSkill ();

		RefreshUi ();
	}

	private void RefreshUnit(Dictionary<int,LinkedList<int>> _clientAttackData){

		Dictionary<int, Unit>.Enumerator enumerator = battle.unitDic.GetEnumerator ();

		while (enumerator.MoveNext ()) {

			int uid = enumerator.Current.Key;

			Unit unit = enumerator.Current.Value;

			HeroStateMachine2 hm;

			if (unitGoDic.ContainsKey (uid)) {

				hm = unitGoDic [uid];

			} else {

				hm = CreateUnitGo (unit);
			}

			hm.UpdateAction (_clientAttackData);
		}

		LinkedListNode<int> node = unitGoList.First;

		while (node != null) {

			LinkedListNode<int> nextNode = node.Next;

			int uid = node.Value;

			if (!battle.unitDic.ContainsKey (uid)) {

				HeroStateMachine2 hm = unitGoDic [uid];

				if (hm.damageTimes == 0 && !hm.isAttacking) {

					RemoveUnitGo (uid);

					unitGoList.Remove (node);
				}
			}

			node = nextNode;
		}
	}

	private void RemoveUnitGo(int _uid){

		HeroStateMachine2 hm = unitGoDic [_uid];

		unitGoDic.Remove (_uid);

		GameObject.Destroy (hm.gameObject);
	}

	private void RefreshSkill(){

		LinkedList<Skill>.Enumerator enumerator = battle.skillList.GetEnumerator ();

		while (enumerator.MoveNext ()) {

			Skill skill = enumerator.Current;

			if (skill.sds.GetEffectRadius () > 0) {

				if (skillGoDic.ContainsKey (skill.uid)) {

					GameObject go = skillGoDic [skill.uid];

					go.transform.localPosition = GetSkillPos (skill);

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

				RemoveSkillGo (uid);

				skillGoList.Remove (node);
			}

			node = nextNode;
		}
	}

	private void RemoveSkillGo(int _uid){

		GameObject go = skillGoDic [_uid];

		go.SetActive (false);

		skillGoDic.Remove (_uid);

		skillPool.AddLast (go);
	}

	private HeroStateMachine2 CreateUnitGo(Unit _unit){
		
		GameObject go = GameObjectFactory.Instance.GetGameObject ("Assets/arts/unitPrefab/orc.prefab", null);

		go.transform.SetParent (unitContainer, false);

		PublicTools.SetLayer(go,unitContainer.gameObject.layer);

//		if(_unit.isMine == battle.clientIsMine){
//
//			go.GetComponent<Renderer> ().material.SetColor ("_Color", Color.blue);
//
//		}else{
//
//			go.GetComponent<Renderer> ().material.SetColor ("_Color", Color.yellow);
//		}

		HeroStateMachine2 hm = go.GetComponent<HeroStateMachine2> ();

		hm.Init (_unit, this);

		unitGoDic.Add(_unit.uid, hm);

		unitGoList.AddLast(_unit.uid);

		return hm;
	}

	public Vector3 GetUnitPos(Unit _unit){

		int fix = battle.clientIsMine ? 1 : -1;

		float y = _unit.sds.GetUnitType() == UnitType.AIR_UNIT ? AIR_UNIT_Y : 0f;

		RVO.Vector2 tmpPos = _unit.pos;

		return new Vector3 ((float)tmpPos.x * fix, y, (float)tmpPos.y * fix);
	}

	private Vector3 GetSkillPos(Skill _skill){

		RVO.Vector2 tmpPos = _skill.pos;

		int fix = battle.clientIsMine ? 1 : -1;

		return new Vector3 ((float)tmpPos.x * fix, 0, (float)tmpPos.y * fix);
	}

	private void CreateSkillGo(Skill _skill){

		int fix = battle.clientIsMine ? 1 : -1;

		GameObject go;

		if (skillPool.Count > 0) {

			go = skillPool.Last.Value;

			skillPool.RemoveLast ();

			go.SetActive (true);

		} else {

			go = GameObjectFactory.Instance.GetGameObject ("Assets/arts/prefab/hero.prefab", null);

			go.transform.SetParent (unitContainer, false);

			PublicTools.SetLayer(go,unitContainer.gameObject.layer);
		}

		if(_skill.unit.isMine == battle.clientIsMine){

			go.GetComponent<Renderer> ().material.SetColor ("_Color", Color.blue);

		}else{

			go.GetComponent<Renderer> ().material.SetColor ("_Color", Color.yellow);
		}

		float scale = (float)_skill.sds.GetEffectRadius () * 2;

		go.transform.localScale = new Vector3 (scale, 0.1f, scale);

		go.transform.localPosition = GetSkillPos (_skill);

		skillGoDic.Add(_skill.uid, go);

		skillGoList.AddLast(_skill.uid);
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
