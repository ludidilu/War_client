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
using UnityEditor;

namespace CreativeSpore
{
    [CustomEditor(typeof(CharAnimationController))]
    public class CharAnimationControllerEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            CharAnimationController targetComp = (CharAnimationController)target;
            if (GUILayout.Button("Open Editor..."))
            {
                CharAnimationWindow.Init(targetComp);
            }

            if (targetComp.IsDataBroken()) targetComp.CreateSpriteFrames();

            EditorGUI.BeginChangeCheck();
            targetComp.SpriteCharSet = (Sprite)EditorGUILayout.ObjectField("SpriteCharSet", targetComp.SpriteCharSet, typeof(Sprite), false);
            targetComp.CharsetType = (CharAnimationController.eCharSetType)EditorGUILayout.EnumPopup("Charset Type", targetComp.CharsetType);
            if( EditorGUI.EndChangeCheck() )
            {
                targetComp.CreateSpriteFrames();
            }

            targetComp.TargetSpriteRenderer = (SpriteRenderer)EditorGUILayout.ObjectField("Target Sprite Render", targetComp.TargetSpriteRenderer, typeof(SpriteRenderer), true);
            targetComp.AnimSpeed = EditorGUILayout.FloatField("Anim Speed", targetComp.AnimSpeed);
			targetComp.AnimFrames = EditorGUILayout.IntField("Anim Frames", targetComp.AnimFrames);
            targetComp.IsPingPongAnim = EditorGUILayout.ToggleLeft("Ping-Pong Anim", targetComp.IsPingPongAnim);
            targetComp.CurrentDir = (CharAnimationController.eDir)EditorGUILayout.EnumPopup("Facing Dir", targetComp.CurrentDir);

            serializedObject.ApplyModifiedProperties();
            if (GUI.changed)
            {
                EditorUtility.SetDirty(targetComp);
            }
        }
    }
}
