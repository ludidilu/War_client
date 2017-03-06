using UnityEngine;
using System.Collections;
using CreativeSpore.RpgMapEditor;
using System.Collections.Generic;

public class MapLayerController : MonoBehaviour
{

    public static MapLayerController m_Instance;

    public static MapLayerController Instance
    {
        get
        {
            return m_Instance;
        }
    }

    private List<string> m_LayerType = new List<string>();
    private List<int> m_EventIds = new List<int>();
    private List<AutoTileMap.MapLayer> m_EventLayers = new List<AutoTileMap.MapLayer>();

    private int m_LayerIndex;

    void Awake()
    {
        m_Instance = this;
    }

    void Start()
    {
        AutoTileMap.Instance.OnMapLoaded += (map) => { HideChildByIndex(0); };
        HideChildByIndex(0);
        Init();
    }

    public void Init()
    {
        m_EventLayers = AutoTileMap.Instance.MapLayers.FindAll(x => x.LayerType == eLayerType.Event);
        HideChildByIndex(0);
    }

    public void CheckLayerChange(int layerIndex)
    {
        if (m_LayerIndex != layerIndex)
        {
            m_LayerIndex = layerIndex;
            HandleLayerChange();
        }
    }

    private void HandleLayerChange()
    {
        Transform child = transform.GetChild(m_LayerIndex);
        HideChildByIndex(m_LayerIndex);
    }

    public void HideChildByIndex(int index)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            bool isEvent = child.name.Split('_').Length > 2;
            if (isEvent)
            {
                if (i == index)
                {
                    child.gameObject.SetActive(true);
                }
                else
                {
                    child.gameObject.SetActive(false);
                }
            }
        }
    }

    public List<int> GetEvenIdsByPosition(Vector3 worldPosition)
    {
        m_EventIds.Clear();

        List<string> layers = GetEventLayersByPosition(worldPosition);

        for (int i = 0; i < layers.Count; i++)
        {
            int curAreaId = int.Parse(layers[i].ToString().Split('_')[1]);
            m_EventIds.Add(curAreaId);
        }
        return m_EventIds;
    }

    public List<string> GetEventLayersByPosition(Vector3 worldPosition)
    {
        m_LayerType.Clear();

        for (int i = 0; i < m_EventLayers.Count; i++)
        {
            AutoTileMap.MapLayer layer = m_EventLayers[i];
            AutoTile centerTile = RpgMapHelper.GetAutoTileByPosition(worldPosition, layer.TileLayerIdx);

            if (centerTile.Id >= 0)
            {
                m_LayerType.Add(layer.Name);
            }
        }
        return m_LayerType;
    }
}
