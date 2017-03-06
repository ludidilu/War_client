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

namespace CreativeSpore
{
	public class ObjectGenerator : MonoBehaviour 
	{
		public GameObject Prefab;
		public float TimerBetweenCreation = 1f;
		public int MaxNumberOfEntities = -1; // < 0 for infinite
		public Vector3 Offset = default(Vector3);

		[SerializeField]
		private List<GameObject> m_listOfEntities = new List<GameObject>();

		void Start()
		{
			StartCoroutine(GenerateObj());
		}
		
		IEnumerator GenerateObj()
		{
			while(true)
			{
				m_listOfEntities.RemoveAll(item => item == null);
				if( MaxNumberOfEntities < 0 || m_listOfEntities.Count < MaxNumberOfEntities )
				{
					GameObject obj = Instantiate(Prefab, transform.position+Offset, Prefab.transform.rotation) as GameObject;
					m_listOfEntities.Add( obj );
				}
				yield return new WaitForSeconds(TimerBetweenCreation);
			}
		}
	}
}
