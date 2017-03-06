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
using CreativeSpore.RpgMapEditor;

namespace CreativeSpore
{
    public class IsoSpriteController : MonoBehaviour
    {

        public SpriteRenderer m_spriteRender;

        private float m_OverlayLayerZ, m_GroundOverlayLayerZ;

        // Use this for initialization
        void Start()
        {
            if (m_spriteRender == null)
            {
                m_spriteRender = GetComponent<SpriteRenderer>();
            }

            m_OverlayLayerZ = AutoTileMap.Instance.FindFirstLayer(eLayerType.Overlay).Depth;
            m_GroundOverlayLayerZ = AutoTileMap.Instance.FindLastLayer(eLayerType.Ground).Depth;

            Vector3 vPos = m_spriteRender.transform.localPosition;
            float f0 = Mathf.Abs(m_OverlayLayerZ - m_GroundOverlayLayerZ);
            float f1 = Mathf.Abs(AutoTileMap.Instance.transform.localPosition.y - transform.localPosition.y) / (AutoTileMap.Instance.MapTileHeight * AutoTileMap.Instance.Tileset.TileWorldHeight);
            vPos.z = m_GroundOverlayLayerZ - f0 * f1;
            m_spriteRender.transform.localPosition = vPos;
        }
    }
}
