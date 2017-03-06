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
using System.Collections.Generic;

namespace CreativeSpore
{
    public class PlayerController : CharBasicController
    {

        public GameObject BulletPrefab;
        public float TimerBlockDirSet = 0.6f;
        public Camera2DController Camera2D;
        public float BulletAngDispersion = 15f;
        public SpriteRenderer ShadowSprite;
        public SpriteRenderer WeaponSprite;
        public int FogSightLength = 5;

        /// <summary>
        /// If player is driving a vehicle, this will be that vehicle
        /// </summary>
        public VehicleCharController Vehicle;

        private FollowObjectBehaviour m_camera2DFollowBehaviour;


        public override void SetVisible(bool value)
        {
            base.SetVisible(value);
            ShadowSprite.enabled = value;
            WeaponSprite.enabled = value;
        }

        // Use this for initialization
        protected override void Start()
        {
            base.Start();
            if (Camera2D == null)
            {
                Camera2D = GameObject.FindObjectOfType<Camera2DController>();
            }

            m_camera2DFollowBehaviour = Camera2D.transform.GetComponent<FollowObjectBehaviour>();
            m_camera2DFollowBehaviour.Target = transform;
        }

        void CreateBullet(Vector3 vPos, Vector3 vDir)
        {
            GameFactory.CreateBullet(gameObject, BulletPrefab, vPos, vDir, 4f);
        }

        private int m_lastTileIdx = -1;
        private int m_lastFogSightLength = 0;

        protected override void Update()
        {
            base.Update();
            m_phyChar.enabled = (Vehicle == null);
            if (Vehicle != null)
            {
                m_animCtrl.IsAnimated = false;
            }
            else
            {
                bool isMoving = (m_phyChar.Dir.sqrMagnitude >= 0.01);
                if (isMoving)
                {
                    //m_phyChar.Dir.Normalize();
                    m_camera2DFollowBehaviour.Target = transform;
                }
                else
                {
                    m_phyChar.Dir = Vector3.zero;
                }
            }

            //int tileIdx = RpgMapHelper.GetTileIdxByPosition(transform.position);

            //if (tileIdx != m_lastTileIdx || m_lastFogSightLength != FogSightLength)
            //{
            //    RpgMapHelper.RemoveFogOfWarWithFade(transform.position, FogSightLength);
            //}

            //m_lastFogSightLength = FogSightLength;
            //m_lastTileIdx = tileIdx;
        }
    }
}
