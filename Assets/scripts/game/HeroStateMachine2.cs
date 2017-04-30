using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using superTween;

public class HeroStateMachine2 :MonoBehaviour {
	
	public const string attackTrigger = "attack";
	public const string walkTrigger = "run";

	private BattleManager battleManager;
	
	[SerializeField]
	public Animator animator;

	private LinkedList<int> damageList = null;

	private int tweenID = -1;

	public Unit hero{ get; private set; }
	
	public bool isAttacking = false;
	
	public int damageTimes;
	
	public UnitSDS sds;
	
	public void Init(Unit _hero, BattleManager _battleManager){
		
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

		Vector3 nowPos = transform.localPosition;

		Action<float> dele = delegate(float obj) {
		
			transform.localPosition = Vector3.Lerp(nowPos,_pos,obj);
		};
		
		tweenID = SuperTween.Instance.To(0, 1, Time.fixedDeltaTime, dele, MoveOver);
	}

	private void MoveOver(){

		tweenID = -1;
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

		} else {

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
		
		if (tweenID != -1) {
			
			SuperTween.Instance.Remove (tweenID);
		}
	}
}
