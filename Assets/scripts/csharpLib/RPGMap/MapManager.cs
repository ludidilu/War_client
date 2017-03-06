using UnityEngine;
using CreativeSpore.RpgMapEditor;
using System;
using System.Collections.Generic;
using CreativeSpore;
using textureFactory;
using publicTools;
using superFunction;
using wwwManager;

public class MapManager : MonoBehaviour
{

    private static MapManager m_Instance;

    public static MapManager Instance
    {
        get
        {
            return m_Instance;
        }
    }

    public const string MEET = "rpg_meet";
    public const string LEAVE_AREA = "leaveArea";
    public const string ENTER_AREA = "enterArea";
    public const string WALKIN = "walkIn";

    [SerializeField]
    private GameObject m_GameWorld;
    [SerializeField]
    private Camera m_MapCamera;
    [SerializeField]
    private CharBasicController m_PlayerController;
    [SerializeField]
    private QuadMasks m_QuadMasks;
    [SerializeField]
    private MapLayerController m_LayerMgr;

    private BoxCollider2D m_PlayerCollider;

    private List<MapObject> m_MapObjects = new List<MapObject>();
    private List<MapObject> m_ActivatedMapObjects = new List<MapObject>();
    private List<MapObject> m_IntersectMapObject = new List<MapObject>();
    private List<MapObject> m_SeletedNearMapObjects = new List<MapObject>();

    private Dictionary<int, MapObject> m_MapObjectDic = new Dictionary<int, MapObject>();
    private int m_MapGameObjectIdentityId;

    private List<int> m_LastEventArea = new List<int>();
    private List<int> m_LeaveEventArea = new List<int>();
    private List<int> m_EnterEventArea = new List<int>();

    private float m_NearDistance;
    private int m_LastTileIdx;

    private int m_DefaultTileId;//出生点
    private Action m_MapLoadCompleted;

    public enum MapPart
    {
        MapRoot,
        Player,
        Camera
    }

    private class MapObject
    {
        public int Id { get; set; }

        public int Index { get; set; }

        public GameObject MapGo { get; set; }

        public BoxCollider2D BoxCollider { get; set; }
    }

    public GameObject PlayerGo
    {
        get { return Player.gameObject; }
    }

    public bool IsPlayerMoving
    {
        get { return Player.PhyCtrl.IsMoving; }
    }

    public GameObject GameWorld
    {
        get
        {
            return m_GameWorld;
        }
        set
        {
            m_GameWorld = value;
        }
    }

    public Camera MapCamera
    {
        get
        {
            return m_MapCamera;
        }
        set
        {
            m_MapCamera = value;
        }
    }

    public CharBasicController Player
    {
        get
        {
            return m_PlayerController;
        }
        set
        {
            m_PlayerController = value;
            SetPlayerBoxCollider();
        }
    }

    public MapLayerController LayerMgr
    {
        get
        {
            return m_LayerMgr;
        }

        set
        {
            m_LayerMgr = value;
        }
    }

    void Awake()
    {
        m_Instance = this;
    }

    void Start()
    {
        m_NearDistance = AutoTileMap.Instance.Tileset.TileWorldWidth * 3f;
        SetPlayerBoxCollider();
    }

    private void SetPlayerBoxCollider()
    {
        m_PlayerCollider = m_PlayerController.GetComponent<BoxCollider2D>();
    }

    public void InitMapData(string mapName, int defaultTileId, Action complete)
    {
        Clear();

        m_DefaultTileId = defaultTileId;
        m_MapLoadCompleted = complete;
        AutoTileMap.Instance.OnMapLoaded += HandleMapLoad;
        LoadMap(mapName);
    }

    private void LoadMap(string mapName)
    {

#if USE_ASSETBUNDLE

		Action<WWW> getXml = delegate(WWW obj) {

			LoadMapXml(obj.text);
		};

		WWWManager.Instance.Load("map/" + mapName + ".xml",getXml);
#else
        TextAsset text = Resources.Load<TextAsset>(mapName);
        LoadMapXml(text.text);
#endif
    }

    private void LoadMapXml(string _str)
    {
        AutoTileMapSerializeData mapData = AutoTileMapSerializeData.LoadFromXmlString(_str);
        AutoTileMap.Instance.MapData.Data.CopyData(mapData);
        AutoTileMap.Instance.LoadMap();
    }

    private void HandleMapLoad(AutoTileMap map)
    {
        m_LastTileIdx = m_DefaultTileId;
        SetPlayerPositionByTileId(m_DefaultTileId);
        SetCameraPositionWithPlayer();

        LayerMgr.Init();
        RefreshPlayerCurrentTileEventAreas();

        if (m_MapLoadCompleted != null)
            m_MapLoadCompleted();
    }

    public void TriggerMeetEvent(int tileId)
    {
        SuperFunction.Instance.DispatchEvent(gameObject, MEET + tileId);
    }

    public void AddGameObject(MapPart part, GameObject go)
    {
        PublicTools.SetLayer(go, GameWorld.layer);

        if (part == MapPart.MapRoot)
        {
            go.transform.SetParent(GameWorld.transform);
        }

        if (part == MapPart.Player)
        {
            go.transform.SetParent(PlayerGo.transform);
        }

        if (part == MapPart.Camera)
        {
            go.transform.SetParent(MapCamera.transform);
        }
    }

    public int AddMapObject(GameObject mapGo, BoxCollider2D collider, int index)
    {
        m_MapGameObjectIdentityId++;

        MapObject mapObj = new MapObject();
        mapObj.Id = m_MapGameObjectIdentityId;
        mapObj.Index = index;
        mapObj.MapGo = mapGo;
        mapObj.BoxCollider = collider;
        m_MapObjectDic.Add(mapObj.Id, mapObj);
        m_MapObjects.Add(mapObj);
        mapGo.layer = gameObject.layer;
        mapGo.transform.position = RpgMapHelper.GetTileCenterPositionById(index);

        ShowMapObject(m_MapGameObjectIdentityId);
        return m_MapGameObjectIdentityId;
    }

    public bool RemoveMapObject(int id)
    {
        if (m_MapObjectDic.ContainsKey(id))
        {
            MapObject mapObj = m_MapObjectDic[id];
            Destroy(mapObj.MapGo);
            m_MapObjects.Remove(mapObj);
            m_MapObjectDic.Remove(id);
            m_ActivatedMapObjects.Remove(mapObj);
            return true;
        }

        return false;
    }

    public GameObject FindMapObject(int id)
    {
        return m_MapObjectDic[id].MapGo;
    }

    public void ShowMapObject(int id)
    {
        MapObject mapObj = m_MapObjectDic[id];

        if (m_ActivatedMapObjects.Contains(mapObj))
        {
            return;
        }

        m_ActivatedMapObjects.Add(mapObj);
        mapObj.MapGo.SetActive(true);
        if (mapObj.BoxCollider != null)
        {
            if (IsIntersectByCollider(mapObj.BoxCollider, m_PlayerCollider))
            {
                AddMapObjectOfIntersectPlayer(mapObj);
            }
        }
    }

    public void HideMapObject(int id)
    {
        MapObject mapObj = m_MapObjectDic[id];
        mapObj.MapGo.SetActive(false);
        m_ActivatedMapObjects.Remove(mapObj);
    }

    public void CheckPlayerTileAction()
    {
        int tileId = RpgMapHelper.GetTileIdxByPosition(PlayerGo.transform.position);

        if (tileId != m_LastTileIdx)
        {
            m_LastTileIdx = tileId;

            SuperFunction.Instance.DispatchEvent(gameObject, WALKIN, tileId);

            CheckPlayerTileEventArea();
            CheckMapObjectLeavePlayer();
        }
    }

    public bool IsIntersectByCollider(BoxCollider2D left, BoxCollider2D right)
    {
        if (left != null && right != null)
        {
            if (right.bounds.Intersects(left.bounds))
                return true;
        }

        return false;
    }

    private void AddMapObjectOfIntersectPlayer(MapObject mapObject)
    {
        if (!m_IntersectMapObject.Contains(mapObject))
            m_IntersectMapObject.Add(mapObject);
    }

    private void CheckMapObjectLeavePlayer()
    {
        for (int i = m_IntersectMapObject.Count - 1; i >= 0; i--)
        {
            MapObject mapObject = m_IntersectMapObject[i];
            if (!m_PlayerCollider.bounds.Intersects(mapObject.BoxCollider.bounds))
            {
                m_IntersectMapObject.RemoveAt(i);
            }
        }
    }

    public int GetPlayerCollidingMapObjectTileId()
    {
        return GetCollidingMapObjectTileId(m_PlayerCollider);
    }

    private int GetCollidingMapObjectTileId(BoxCollider2D collider)
    {
        List<MapObject> mapObjects = GetNearMapObjects(collider.transform.position);

        for (int i = 0; i < mapObjects.Count; i++)
        {
            MapObject mapObject = mapObjects[i];

            if (mapObject.MapGo.gameObject.activeSelf)
            {
                if (IsIntersectByCollider(mapObject.BoxCollider, collider))
                {
                    if (!m_IntersectMapObject.Contains(mapObject))
                    {
                        return mapObject.Index;
                    }
                }
            }
        }

        return -1;
    }

    private List<MapObject> GetNearMapObjects(Vector3 position)
    {
        return GetMapObjectsByRadius(position, m_NearDistance);
    }

    private List<MapObject> GetMapObjectsByRadius(Vector3 position, float radius)
    {
        m_SeletedNearMapObjects.Clear();
        for (int i = 0; i < m_ActivatedMapObjects.Count; i++)
        {
            MapObject obj = m_ActivatedMapObjects[i];

            if (Vector3.Distance(obj.MapGo.transform.position, position) < m_NearDistance)
            {
                m_SeletedNearMapObjects.Add(obj);
            }
        }

        return m_SeletedNearMapObjects;
    }

    public List<int> GetMapObjectIdsByRadius(Vector3 position, float radius)
    {
        List<MapObject> mapObjects = GetMapObjectsByRadius(position, m_NearDistance);
        List<int> mapObjectIds = new List<int>();

        for (int i = 0; i < mapObjects.Count; i++)
        {
            mapObjectIds.Add(mapObjects[i].Id);
        }

        return mapObjectIds;
    }

    public void RefreshPlayerCurrentTileEventAreas()
    {
        RemoveAllPlayerEventArea();
        CheckPlayerTileEventArea();
    }

    private void RemoveAllPlayerEventArea()
    {
        CheckTileEventArea(new Vector3(-1f, -1f, 0));
    }

    private void CheckPlayerTileEventArea()
    {
        CheckTileEventArea(PlayerGo.transform.position);
    }

    private void CheckTileEventArea(Vector3 position)
    {
        m_LeaveEventArea.Clear();
        m_EnterEventArea.Clear();

        List<int> eventIds = LayerMgr.GetEvenIdsByPosition(position);

        for (int i = 0; i < eventIds.Count; i++)
        {
            int curAreaId = eventIds[i];
            if (!m_LastEventArea.Contains(curAreaId))
            {
                m_EnterEventArea.Add(curAreaId);
            }
        }

        for (int i = 0; i < m_LastEventArea.Count; i++)
        {
            int oldAreaId = m_LastEventArea[i];
            if (!eventIds.Contains(oldAreaId))
            {
                m_LeaveEventArea.Add(oldAreaId);
            }
        }

        for (int i = m_LastEventArea.Count - 1; i >= 0; i--)
        {
            int AreaId = m_LastEventArea[i];

            if (m_LeaveEventArea.Contains(AreaId))
            {
                m_LastEventArea.RemoveAt(i);
            }
        }

        m_LastEventArea.AddRange(m_EnterEventArea);

        if (m_LeaveEventArea.Count > 0)
        {

            SuperFunction.Instance.DispatchEvent(gameObject, LEAVE_AREA, m_LeaveEventArea);
        }

        if (m_EnterEventArea.Count > 0)
        {

            SuperFunction.Instance.DispatchEvent(gameObject, ENTER_AREA, m_EnterEventArea);
        }
    }

    public List<int> GetPlayerCurrentTileEventids()
    {
        return LayerMgr.GetEvenIdsByPosition(PlayerGo.transform.position);
    }

    public void AddQuadMaskTarget(Transform _target)
    {
        m_QuadMasks.AddTarget(_target);
    }

    public void RemoveQuadMaskTarget(Transform _target)
    {
        m_QuadMasks.RemoveTarget(_target);
    }

    public void ShowMask()
    {
        m_QuadMasks.gameObject.SetActive(true);
    }

    public void HideMask()
    {
        m_QuadMasks.gameObject.SetActive(false);
    }

    public void SetMaskRadios(float _radios)
    {
        m_QuadMasks.SetLightRadios(_radios);
    }

    public void SetCameraActive(bool bl)
    {
        MapCamera.gameObject.SetActive(bl);
    }

    private void SetCameraPositionWithPlayer()
    {
        Vector3 playerPos = PlayerGo.transform.position;
        MapCamera.transform.position = new Vector3(playerPos.x, playerPos.y, MapCamera.transform.position.z);
    }

    public void UpdatePlayerMovement(float x, float y)
    {
        Player.UpdateMovement(x, y);
    }

    public void SetPlayerMoveSpeed(float speed)
    {
        Player.PhyCtrl.MaxSpeed = speed;
    }

    public void FixMoveSpeed(float multiply)
    {

        if (multiply == 0)
        {
            return;
        }

        Player.PhyCtrl.MaxSpeed *= multiply;
    }

    public void SetPlayerPositionByTileId(int tileId)
    {
        PlayerGo.transform.position = RpgMapHelper.GetTileCenterPositionById(tileId);
    }

    private void Clear()
    {
        RemoveAllPlayerEventArea();

        m_MapObjects.Clear();
        m_ActivatedMapObjects.Clear();
        m_MapObjectDic.Clear();
        m_SeletedNearMapObjects.Clear();
        m_LastEventArea.Clear();
        m_IntersectMapObject.Clear();

        AutoTileMap.Instance.OnMapLoaded -= HandleMapLoad;
    }
}
