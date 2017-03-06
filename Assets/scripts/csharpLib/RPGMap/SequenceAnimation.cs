using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SequenceAnimation : MonoBehaviour
{
    public float AnimSpeed = 3.5f;

    public int AniamtionFrameCount = 12;

    public Sprite SpriteSet;

    public SpriteRenderer TargetSpriteRenderer;

    private List<Sprite> m_spriteFrames = new List<Sprite>();

    private static Vector2 m_Pivot = new Vector2(0.5f, 0.5f);
    private int m_internalFrame;

    public int CurrentFrame
    {
        get
        {
            return m_internalFrame;
        }
    }

    private bool IsAnimated;
    private int AnimFrames = 3;
    private float m_curFrameTime;


    // Use this for initialization
    void Start()
    {
        if (TargetSpriteRenderer == null)
            TargetSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Play()
    {
        IsAnimated = true;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAnim(Time.deltaTime);
    }

    public void UpdateAnim(float dt)
    {
        if (TargetSpriteRenderer == null || TargetSpriteRenderer.sprite == null)
            return;

        if (IsAnimated)
        {
            m_curFrameTime += dt * AnimSpeed;
            while (m_curFrameTime >= 1f)
            {
                m_curFrameTime -= 1f;
                m_internalFrame++;
                m_internalFrame %= AniamtionFrameCount;

                if (m_internalFrame == 0)
                {
                    IsAnimated = false;
                }
            }
        }
        else
        {
            m_internalFrame = 0;
        }

        TargetSpriteRenderer.sprite = GetCurrentSprite();
    }

    public Sprite GetCurrentSprite()
    {
        return m_spriteFrames[CurrentFrame];
    }

}
