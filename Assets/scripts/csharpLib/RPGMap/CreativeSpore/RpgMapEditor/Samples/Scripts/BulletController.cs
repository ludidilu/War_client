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
using CreativeSpore.RpgMapEditor;

namespace CreativeSpore
{
	public class BulletController : MonoBehaviour {

		public float Speed = 0.1f;
		public Vector3 Dir = new Vector3();
		public float TimeToLive = 5f;
        public float DamageQty = 0.5f;

		public GameObject OnDestroyFx;
		public bool IsDestroyOnCollision = true;

		void Start()
		{
			Destroy( transform.gameObject, TimeToLive);
		}

		// Update is called once per frame
		void Update () 
		{
			if( AutoTileMap.Instance.GetAutotileCollisionAtPosition( transform.position ) == eTileCollisionType.BLOCK )
			{
				Destroy( transform.gameObject );
			}
			transform.position += Speed * Dir * Time.deltaTime;
		}

		void OnDestroy()
		{
			if( OnDestroyFx != null )
			{
				Instantiate( OnDestroyFx, transform.position, transform.rotation );
			}
		}
		
		void OnTriggerStay(Collider other) 
		{
			if( IsDestroyOnCollision && other.attachedRigidbody && (other.gameObject.layer != gameObject.layer) )
			{
				//apply damage here
                DamageData.ApplyDamage(other.attachedRigidbody.gameObject, DamageQty, Dir);
				Destroy(gameObject);
			}
		}
	}
}
