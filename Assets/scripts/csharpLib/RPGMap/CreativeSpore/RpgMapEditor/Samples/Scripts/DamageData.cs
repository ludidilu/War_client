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
	public class DamageData 
	{

		public enum eDmgType
		{
			NORMAL, // general damage
			SHIELD // applied to shield
		}

		public eDmgType Type = eDmgType.NORMAL;
		public float 	Quantity	= 0.0f;
		public Vector2	Force		= Vector2.zero;

		public void ApplyDamage( GameObject _obj )
		{
			DamageBehaviour damageBehaviour = _obj.GetComponent<DamageBehaviour>();
			if (damageBehaviour)
			{
				damageBehaviour.ApplyDamage(this);
			}
		}

		public static void ApplyDamage( GameObject _obj, float _quantity, Vector2 _force = default(Vector2), eDmgType _type = eDmgType.NORMAL )
		{
			DamageData damageData = new DamageData(){ Quantity = _quantity, Force = _force, Type = _type };
			damageData.ApplyDamage( _obj );
		}
	}
}
