using UnityEngine;
using superFunction;
using System;

public class MapObjects : MonoBehaviour
{
    public BoxCollider2D BoxCollider { get; private set; }

    public int TileIndex { get; private set; }

    private GameObject eventGo;

    private Action m_OnEnable;

    private Action m_OnDisable;

    private Action m_OnMeet;

    void Start()
    {

    }

    void OnEnable()
    {
        if (m_OnEnable != null)
        {
            m_OnEnable();
        }
    }

    void OnDisable()
    {
        if (m_OnDisable != null)
        {
            m_OnDisable();
        }
    }

    public void Init(GameObject _eventGo, int _pos)
    {
        eventGo = _eventGo;

        TileIndex = _pos;

        SuperFunction.Instance.AddEventListener(eventGo, MapManager.MEET + TileIndex, Meet);
    }

    public void SetBoxCollider(BoxCollider2D bc)
    {
        BoxCollider = bc;
    }

    public void RegisterMeetEvent(Action callback)
    {
        m_OnMeet = callback;
    }

    public void RegisterOnEnableEvent(Action callback)
    {
        m_OnEnable = callback;
    }

    public void RegisterOnDisableEvent(Action callback)
    {
        m_OnDisable = callback;
    }

    private void Meet(int index, params object[] array)
    {
        if (m_OnMeet != null)
        {
            m_OnMeet();
        }
    }

    void OnDestroy()
    {
        SuperFunction.Instance.RemoveEventListener(eventGo, MapManager.MEET + TileIndex, Meet);
    }
}
