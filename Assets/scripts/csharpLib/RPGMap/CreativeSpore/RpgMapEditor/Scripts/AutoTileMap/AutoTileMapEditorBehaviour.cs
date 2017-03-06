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
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace CreativeSpore.RpgMapEditor
{
	[ExecuteInEditMode]
	public class AutoTileMapEditorBehaviour : MonoBehaviour 
	{
		AutoTileMap MyAutoTileMap;

	#if UNITY_EDITOR

		void Awake()
		{
			//hideFlags = HideFlags.HideInInspector;
			MyAutoTileMap = GetComponent<AutoTileMap>();

			OnRenderObject();
		}
		
		void OnRenderObject() 
		{
			if( MyAutoTileMap == null )
			{
				MyAutoTileMap = GetComponent<AutoTileMap>();
			}

			if( MyAutoTileMap != null )
			{
				if( !MyAutoTileMap.IsInitialized )
				{
					if( MyAutoTileMap.CanBeInitialized() )
					{
						MyAutoTileMap.LoadMap();
					}
				}
			}
		}
	#endif
	}
}
