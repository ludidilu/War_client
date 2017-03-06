/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

using UnityEngine;
using System.Collections;

namespace CreativeSpore
{
	public class DamageBehaviour : MonoBehaviour 
	{
		public float Health = 1f;
		public GameObject FXDeathPrefab;
		public GameObject RandomDrop;
		public float GodModeTimer = 0.3f;

		private SpriteRenderer m_sprRender;
		private MovingBehaviour m_movingBehaviour;

		private float m_timerGodMode;
		private DamageData m_lastDamageData;

		// Use this for initialization
		void Start () 
		{
			m_sprRender = GetComponentInChildren<SpriteRenderer>();
			m_movingBehaviour = GetComponent<MovingBehaviour>();
		}
		
		// Update is called once per frame
		void Update () 
		{
			if( m_timerGodMode > 0 )
			{
				m_timerGodMode -= Time.deltaTime;
				m_sprRender.color = Time.frameCount % 2 == 0? Color.red : Color.white;

				if( m_movingBehaviour != null )
				{
					m_movingBehaviour.ApplyForce( m_lastDamageData.Force * m_movingBehaviour.MaxForce );
				}
			}
			else
			{
				m_sprRender.color = Color.white;
			}
		}

		public void ApplyDamage( DamageData _damageData )
		{
			if( m_timerGodMode <= 0 )
			{
				m_timerGodMode = GodModeTimer;
				m_lastDamageData = _damageData;

				//AudioSource.PlayClipAtPoint(SoundLibController.GetInstance().GetSound("hurtEnemy_00"), transform.position); 
							
				Health-=_damageData.Quantity;
				if( Health <= 0 )
				{
					Destroy( gameObject );
					if( FXDeathPrefab )
					{
						GameObject fxDeath = Instantiate( FXDeathPrefab, transform.position, FXDeathPrefab.transform.rotation ) as GameObject;
						Destroy( fxDeath, 3f);
					}
					if( RandomDrop )
					{
						Instantiate( RandomDrop, transform.position, transform.rotation );
					}
				}
			}
		}
	}
}
