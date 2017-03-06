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
	public class AnimationController : MonoBehaviour 
	{

		public enum eAnimType
		{
			LOOP,
			ONESHOOT
		}
		
		public List<Sprite> SpriteFrames = new List<Sprite>();
		
		public eAnimType AnimType = eAnimType.LOOP;
		public bool IsAnimated = true;
		public float AnimSpeed = 0.2f;
		public bool IsDestroyedOnAnimEnd = false;
		public float CurrentFrame
		{
			get { return m_currAnimFrame; }
		}
		
		private float m_currAnimFrame = 0f;
		
		
		private SpriteRenderer m_spriteRenderer;
		
		// Use this for initialization
		void Start () 
		{
			m_spriteRenderer = GetComponentInChildren<SpriteRenderer>();
		}
		
		// Update is called once per frame
		void Update() 
		{
			if (IsAnimated)
			{
				m_currAnimFrame += AnimSpeed * Time.deltaTime;

				if( AnimType == eAnimType.LOOP )
				{
					while (m_currAnimFrame >= SpriteFrames.Count)
						m_currAnimFrame -= SpriteFrames.Count;
				}
				else if(m_currAnimFrame >= SpriteFrames.Count) 
				{
					if( IsDestroyedOnAnimEnd )
					{
						Destroy(transform.gameObject);
						return;
					}
					else
					{
						m_currAnimFrame = 0f;
						IsAnimated = false;
					}
				}
			}
			else
			{
				m_currAnimFrame = 0.9f;
			}
			
			m_spriteRenderer.sprite = SpriteFrames[(int)m_currAnimFrame];
		}
	}
}
