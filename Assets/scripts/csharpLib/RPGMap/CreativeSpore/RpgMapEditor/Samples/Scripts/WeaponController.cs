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
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

namespace CreativeSpore
{
	public class WeaponController : MonoBehaviour {
		/*
		0.06	0.08	-0.1
		0.04	0.08	-0.1
		0.02	0.07	-0.1

		-0.1	0.03	-0.1
		-0.09	0.05	-0.1
		-0.07	0.04	-0.1
		*/

		public Vector3[] vOffDirRight = new Vector3[3]  
		{ 
			new Vector3( 0.06f, 0.08f, -0.0001f ),
			new Vector3( 0.04f, 0.08f, -0.0001f ),
			new Vector3( 0.02f, 0.07f, -0.0001f ),
		};

		public Vector3[] vOffDirDown = new Vector3[3]  
		{ 
			new Vector3( -0.1f, 0.03f, -0.0001f ),
			new Vector3( -0.09f, 0.05f, -0.0001f ),
			new Vector3( -0.07f, 0.04f, -0.0001f ),
		};

		public SpriteRenderer WeaponSprite;
		public Sprite WeaponSpriteHorizontal;
		public Sprite WeaponSpriteVertical;

		private CharAnimationController m_charAnimCtrl;

		// Use this for initialization
		void Start () 
		{
			m_charAnimCtrl = GetComponent<CharAnimationController>();
		}
		
		// Update is called once per frame
		void LateUpdate () 
		{
			WeaponSprite.sprite = WeaponSpriteHorizontal;
			Quaternion qRot = WeaponSprite.transform.localRotation;

			if (m_charAnimCtrl.CurrentDir == CharAnimationController.eDir.RIGHT)
			{
				qRot.eulerAngles = new Vector3(0f, 0f, 0f);
				WeaponSprite.transform.localPosition = vOffDirRight [(int)m_charAnimCtrl.CurrentFrame];
			} 
			else if (m_charAnimCtrl.CurrentDir == CharAnimationController.eDir.LEFT)
			{
				qRot.eulerAngles = new Vector3(0f, 180f, 0f);
				Vector3 vOff = vOffDirRight [(int)m_charAnimCtrl.CurrentFrame];
				vOff.x = -vOff.x;
				vOff.z = -vOff.z;
				WeaponSprite.transform.localPosition = vOff;
			} 
			else if (m_charAnimCtrl.CurrentDir == CharAnimationController.eDir.DOWN)
			{
				qRot.eulerAngles = new Vector3(0f, 0f, 270f);
				WeaponSprite.sprite = WeaponSpriteVertical;			
				WeaponSprite.transform.localPosition = vOffDirDown[(int)m_charAnimCtrl.CurrentFrame];
			}
			else // UP
			{
				qRot.eulerAngles = new Vector3(0f, 180f, 90f);
				Vector3 vOff = vOffDirDown [(int)m_charAnimCtrl.CurrentFrame];
				vOff.x = -vOff.x;
				vOff.z = -vOff.z;
				vOff.y = vOff.y + 0.08f;
				WeaponSprite.transform.localPosition = vOff;
			}
			
			WeaponSprite.transform.localRotation = qRot;
		}
	}
}
