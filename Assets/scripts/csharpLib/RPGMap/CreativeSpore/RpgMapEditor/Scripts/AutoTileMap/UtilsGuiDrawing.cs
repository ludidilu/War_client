/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CreativeSpore.RpgMapEditor
{
	public class UtilsGuiDrawing
	{
		public static void DrawRectWithOutline( Rect rect, Color color, Color colorOutline )
		{
	#if UNITY_EDITOR
			Vector3[] rectVerts = { new Vector3(rect.x, rect.y, 0), 
				new Vector3(rect.x + rect.width, rect.y, 0), 
				new Vector3(rect.x + rect.width, rect.y + rect.height, 0), 
				new Vector3(rect.x, rect.y + rect.height, 0) };
			Handles.DrawSolidRectangleWithOutline(rectVerts, color, colorOutline);
	#else
			Texture2D texture = new Texture2D(1, 1);
			texture.SetPixel(0,0,colorOutline);
			texture.Apply();

			Rect rLine = new Rect( rect.x, rect.y, rect.width, 1 );
			GUI.DrawTexture(rLine, texture);
			rLine.y = rect.y + rect.height - 1;
			GUI.DrawTexture(rLine, texture);
			rLine = new Rect( rect.x, rect.y+1, 1, rect.height-2 );
			GUI.DrawTexture(rLine, texture);
			rLine.x = rect.x + rect.width - 1;
			GUI.DrawTexture(rLine, texture);

			rect.x += 1;
			rect.y += 1;
			rect.width -= 2;
			rect.height -= 2;
			texture.SetPixel(0,0,color);
			texture.Apply();
			GUI.DrawTexture(rect, texture);
	#endif
		}
	}
}
