﻿using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using superTween;
using publicTools;
using gameObjectFactory;

public class HeroStateMachine2 :MonoBehaviour {
	
	public const string attackTrigger = "attack";
	public const string walkTrigger = "run";

	private BattleManager battleManager;
	
	[SerializeField]
	public Animator animator;

	private LinkedList<int> damageList = null;

	private int xTweenID = -1;

	private int zTweenID = -1;

	public Unit hero{ get; private set; }
	
	public bool isAttacking = false;
	
	public int damageTimes;
	
	public UnitSDS sds;

	public void Init(Unit _hero, BattleManager _battleManager){

		UnitSDS sds = _hero.sds as UnitSDS;

		float scale = sds.scale;

		transform.localScale = new Vector3 (scale, scale, scale);

		transform.localPosition = _battleManager.GetUnitPos(_hero);
		
		damageTimes = 0;
		
		hero = _hero;

		battleManager = _battleManager;
	}

	private void Move(float _prefX, float _prefY) {
		
		float temp = Mathf.Atan2(_prefY, _prefX);

		transform.eulerAngles = new Vector3(0,90 - temp * 180 / Mathf.PI,0);

		StateMachineChange(walkTrigger);
	}

	private void StateMachineChange(string _action)
	{
		animator.ResetTrigger(walkTrigger);
		
		animator.ResetTrigger(attackTrigger);
		
		animator.SetTrigger(_action);
	}
	
	private void Attack(float _x,float _y) {

		float y = _y - transform.localPosition.z;
		float x = _x - transform.localPosition.x;
		
		float temp = Mathf.Atan2(y,x); 

		transform.eulerAngles = new Vector3(0,90 - temp * 180 / Mathf.PI,0);

		StateMachineChange(attackTrigger);
	}

	public void SetPos(Vector3 _pos){
		
		xTweenID = SuperTween.Instance.To(transform.localPosition.x,_pos.x,Time.fixedDeltaTime,ChangePosX,null);
		
		zTweenID = SuperTween.Instance.To(transform.localPosition.z,_pos.z,Time.fixedDeltaTime,ChangePosZ,MoveOver);
	}

	private void ChangePosX(float _v){

		transform.localPosition = new Vector3(_v,transform.localPosition.y,transform.localPosition.z);
	}

	private void ChangePosZ(float _v){

		transform.localPosition = new Vector3(transform.localPosition.x,transform.localPosition.y,_v);
	}

	private void MoveOver(){

		xTweenID = zTweenID = -1;
	}

	public void UpdateAction(Dictionary<int,LinkedList<int>> _attackData){
		
		SetPos (battleManager.GetUnitPos (hero));

		if (_attackData != null && _attackData.ContainsKey (hero.uid)) {

			DoAttackCommand ();

			damageList = _attackData [hero.uid];

			LinkedList<int>.Enumerator enumerator = damageList.GetEnumerator ();

			while (enumerator.MoveNext ()) {

				HeroStateMachine2 hm = battleManager.unitGoDic [enumerator.Current];

				hm.damageTimes++;

				if(!isAttacking){

					isAttacking = true;

					Attack(hm.transform.localPosition.x, hm.transform.localPosition.z);
				}
			}

		} else if (hero.targetUid == -1 && !isAttacking) {

			if (hero.prefVelocity != RVO.Vector2.zero) {

				float posFix = battleManager.battle.clientIsMine ? 1 : -1;

				Move ((float)hero.prefVelocity.x * posFix, (float)hero.prefVelocity.y * posFix);
			}
		}
	}
	
	public void Hit(){
		
		isAttacking = false;

		DoAttackCommand ();
	}

	public void Shoot(){

		isAttacking = false;

		GameObject go = GameObjectFactory.Instance.GetGameObject ("Assets/arts/prefab/missile.prefab", null);

		go.transform.SetParent(battleManager.unitContainer,false);

		PublicTools.SetLayer(go,battleManager.unitContainer.gameObject.layer);

		BallisticControl bc = go.GetComponent<BallisticControl>();

		LinkedList<int> tmpDamageList = damageList;

		damageList = null;

		Action dele = delegate() {
		
			GameObject.Destroy(go);

			LinkedList<int>.Enumerator enumerator = tmpDamageList.GetEnumerator ();

			while (enumerator.MoveNext ()) {

				battleManager.unitGoDic [enumerator.Current].damageTimes--;
			}
		};

		Vector3 targetPos = battleManager.unitGoDic[tmpDamageList.First.Value].transform.localPosition;

		bc.Fly(transform.localPosition,targetPos,dele);
	}
	
	private void DoAttackCommand(){
		
		if(damageList != null){

			LinkedList<int>.Enumerator enumerator = damageList.GetEnumerator ();

			while (enumerator.MoveNext ()) {

				battleManager.unitGoDic [enumerator.Current].damageTimes--;
			}
			
			damageList = null;
		}
	}

	void OnDestroy(){
		
		if (xTweenID != -1) {
			
			SuperTween.Instance.Remove (xTweenID);

			SuperTween.Instance.Remove (zTweenID);
		}
	}
}
