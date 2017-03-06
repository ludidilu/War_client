using UnityEngine;
using System.Collections;
using CreativeSpore.RpgMapEditor;

public class PlayerBounds : MonoBehaviour
{
    public SpriteRenderer m_SpriteRenderer;

    private Sprite m_Sprite;
    private Vector2 m_ColliderSize;


    // Use this for initialization
    void Start()
    {
        m_Sprite = m_SpriteRenderer.sprite;
        m_ColliderSize = new Vector2(m_Sprite.bounds.size.x * Mathf.Abs(m_SpriteRenderer.transform.localScale.x), m_Sprite.bounds.size.y * Mathf.Abs(m_SpriteRenderer.transform.localScale.y)) / 2;
    }

    private void LateUpdate()
    {
        Rect mapBounds = new Rect();
        mapBounds.width = AutoTileMap.Instance.MapTileWidth * AutoTileMap.Instance.Tileset.TileWorldWidth;
        mapBounds.height = AutoTileMap.Instance.MapTileHeight * AutoTileMap.Instance.Tileset.TileWorldHeight;
        mapBounds.x = AutoTileMap.Instance.transform.position.x;
        mapBounds.y = AutoTileMap.Instance.transform.position.y;

        mapBounds.y -= mapBounds.height;

        // when the player reaches a bound, we apply the specified bound behavior
        if (transform.position.y + m_ColliderSize.y > mapBounds.yMax)
            ApplyBoundsBehavior(new Vector2(transform.position.x, mapBounds.yMax - m_ColliderSize.y));

        if (transform.position.y - m_ColliderSize.y < mapBounds.yMin)
            ApplyBoundsBehavior(new Vector2(transform.position.x, mapBounds.yMin + m_ColliderSize.y));

        if (transform.position.x + m_ColliderSize.x > mapBounds.xMax)
            ApplyBoundsBehavior(new Vector2(mapBounds.xMax - m_ColliderSize.x, transform.position.y));

        if (transform.position.x - m_ColliderSize.x < mapBounds.xMin)
            ApplyBoundsBehavior(new Vector2(mapBounds.xMin + m_ColliderSize.x, transform.position.y));
    }

    private void ApplyBoundsBehavior(Vector2 constrainedPosition)
    {
        transform.position = constrainedPosition;
    }
}
