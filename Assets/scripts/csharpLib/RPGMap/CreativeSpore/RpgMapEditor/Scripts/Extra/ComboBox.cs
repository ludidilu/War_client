/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

using UnityEngine;

public class ComboBox
{
    public Rect Rect;
    public bool IsDropDownListVisible = false;

    private static bool forceToUnShow = false;
    private static int useControlID = -1;
    private int selectedItemIndex = 0;

    private GUIContent buttonContent;
    private GUIContent[] listContent;
    private string buttonStyle;
    private string boxStyle;
    private GUIStyle listStyle;

    public ComboBox(Rect rect, int selectedIdx, string[] options, GUIStyle listStyle)
    {
        GUIContent[] listContent = new GUIContent[options.Length];
        for (int i = 0; i < options.Length; ++i)
        {
            listContent[i] = new GUIContent(options[i]);
        }

        this.Rect = rect;
        this.buttonContent = listContent[selectedIdx];
        this.listContent = listContent;
        this.buttonStyle = "button";
        this.boxStyle = "box";
        this.listStyle = listStyle;
    }

    public ComboBox(Rect rect, GUIContent buttonContent, GUIContent[] listContent, GUIStyle listStyle)
    {
        this.Rect = rect;
        this.buttonContent = buttonContent;
        this.listContent = listContent;
        this.buttonStyle = "button";
        this.boxStyle = "box";
        this.listStyle = listStyle;
    }

    public ComboBox(Rect rect, GUIContent buttonContent, GUIContent[] listContent, string buttonStyle, string boxStyle, GUIStyle listStyle)
    {
        this.Rect = rect;
        this.buttonContent = buttonContent;
        this.listContent = listContent;
        this.buttonStyle = buttonStyle;
        this.boxStyle = boxStyle;
        this.listStyle = listStyle;
    }

    public int Show()
    {
        if (forceToUnShow)
        {
            forceToUnShow = false;
            IsDropDownListVisible = false;
        }

        bool done = false;
        int controlID = GUIUtility.GetControlID(FocusType.Passive);

        switch (Event.current.GetTypeForControl(controlID))
        {
            case EventType.mouseUp:
                {
                    if (IsDropDownListVisible)
                    {
                        done = true;
                    }
                }
                break;
        }

        if (GUI.Button(Rect, buttonContent, buttonStyle))
        {
            if (useControlID == -1)
            {
                useControlID = controlID;
                IsDropDownListVisible = false;
            }

            if (useControlID != controlID)
            {
                forceToUnShow = true;
                useControlID = controlID;
            }
            IsDropDownListVisible = true;
        }

        if (IsDropDownListVisible)
        {
            Rect listRect = new Rect(Rect.x, Rect.y + listStyle.CalcHeight(listContent[0], 1.0f),
                      Rect.width, listStyle.CalcHeight(listContent[0], 1.0f) * listContent.Length);

            GUI.Box(listRect, "", boxStyle);
            int newSelectedItemIndex = GUI.SelectionGrid(listRect, selectedItemIndex, listContent, 1, listStyle);
            if (newSelectedItemIndex != selectedItemIndex)
            {
                SelectedItemIndex = newSelectedItemIndex;
            }
        }

        if (done)
            IsDropDownListVisible = false;

        return selectedItemIndex;
    }

    public int SelectedItemIndex
    {
        get
        {
            return selectedItemIndex;
        }
        set
        {
            selectedItemIndex = value;
            buttonContent = listContent[selectedItemIndex];
        }
    }
}
