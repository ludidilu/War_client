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
	public class FollowObjectBehaviour : MonoBehaviour {

		public float DampTime = 0.15f;
		public Transform Target;

		private Vector3 velocity = Vector3.zero;
        private Camera m_camera;

        void Start()
        {
            m_camera = GetComponent<Camera>();
        }
		
		// Update is called once per frame
		void Update () 
		{
			if (Target)
			{
                Vector3 point = m_camera.WorldToViewportPoint(Target.position);
                Vector3 delta = Target.position - m_camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z)); //(new Vector3(0.5, 0.5, point.z));
				Vector3 destination = transform.position + delta;
				transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, DampTime);
			}		
		}
	}
}
