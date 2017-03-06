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
	public class GameFactory : MonoBehaviour
	{
		public static BulletController CreateBullet( GameObject caller, GameObject bulletPrefab, Vector3 vPos, Vector3 vDir, float speed, float dmgQty = 0.5f )
		{
			GameObject bullet = Instantiate(bulletPrefab, vPos, bulletPrefab.transform.rotation) as GameObject;
			
			// set friendly tag, to avoid collision with this layers
			bullet.layer = caller.layer;
			
			BulletController bulletCtrl = bullet.GetComponent<BulletController>();		
			bulletCtrl.Dir = vDir;
			bulletCtrl.Speed = speed;
            bulletCtrl.DamageQty = dmgQty;
			
			return bulletCtrl;
		}

		public static AnimationController CreateAnimation( GameObject prefab, Vector3 vPos )
		{
			GameObject obj = Instantiate(prefab, vPos, prefab.transform.rotation) as GameObject;		
			AnimationController ctrl = obj.GetComponent<AnimationController>();			
			return ctrl;
		}
	}
}
