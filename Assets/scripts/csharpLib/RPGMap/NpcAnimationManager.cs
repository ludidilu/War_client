using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CreativeSpore.RpgMapEditor;

public class NpcAnimationManager : MonoBehaviour
{
    private Dictionary<int, List<SequenceAnimation>> m_AnimationGroup = new Dictionary<int, List<SequenceAnimation>>();
    private List<SequenceAnimation> m_Animations = new List<SequenceAnimation>();
    private List<int> m_GroupIds = new List<int>();

    private Transform m_Player;
    private float m_PlayCheckTime = 3f;//播放检查间隔
    private float m_PlayRate = 0.7f;//播放比率
    private float m_NearDistance;

    public float PlayCheckTime
    {
        get
        {
            return m_PlayCheckTime;
        }

        set
        {
            m_PlayCheckTime = value;
        }
    }

    public float PlayRate
    {
        get
        {
            return m_PlayRate;
        }

        set
        {
            m_PlayRate = value;
        }
    }

    // Use this for initialization
    void Start()
    {
        m_NearDistance = AutoTileMap.Instance.Tileset.TileWorldWidth * 10f;
    }

    public void StartPlayAniamtions()
    {
        StartCoroutine(CheckAniamtionPlay());
    }

    public void StopPlayAniamtions()
    {
        StopCoroutine(CheckAniamtionPlay());
    }

    public void AddNpcAnimation(SequenceAnimation sa)
    {
        m_Animations.Add(sa);
    }

    public void AddNpcAnimation(int groupId, SequenceAnimation sa)
    {
        if (!m_AnimationGroup.ContainsKey(groupId))
        {
            m_AnimationGroup[groupId] = new List<SequenceAnimation>();
            m_GroupIds.Add(groupId);
        }

        m_AnimationGroup[groupId].Add(sa);
    }

    public void SetTargetTransform(Transform transf)
    {
        m_Player = transf;
    }

    private IEnumerator CheckAniamtionPlay()
    {
        while (true)
        {
            yield return new WaitForSeconds(PlayCheckTime);

            CheckAnimations();

            for (int i = 0; i < m_Animations.Count; i++)
            {
                SequenceAnimation sa = m_Animations[i];

                if (CanPlay())
                {
                    if (CheckIsNear(sa.transform.position))
                        sa.Play();
                }
            }

            for (int i = 0; i < m_GroupIds.Count; i++)
            {
                int id = m_GroupIds[i];

                if (CanPlay())
                {
                    PlayAnimationGroup(id);
                }
            }
        }
    }

    private void CheckAnimations()
    {
        for (int i = m_Animations.Count - 1; i >= 0; i--)
        {
            SequenceAnimation sa = m_Animations[i];

            if (sa == null)
            {
                m_Animations.RemoveAt(i);
            }
        }

        for (int i = m_GroupIds.Count - 1; i >= 0; i--)
        {
            int id = m_GroupIds[i];
            List<SequenceAnimation> animations = m_AnimationGroup[id];
            for (int j = animations.Count - 1; j >= 0; j--)
            {
                SequenceAnimation sa = animations[j];
                if (sa == null)
                {
                    animations.RemoveAt(j);
                }

                if (animations.Count == 0)
                {
                    m_AnimationGroup.Remove(id);
                    m_GroupIds.RemoveAt(i);
                }
            }
        }
    }

    private void PlayAnimationGroup(int id)
    {
        List<SequenceAnimation> animations = m_AnimationGroup[id];
        for (int i = 0; i < animations.Count; i++)
        {
            SequenceAnimation sa = animations[i];
            if (CheckIsNear(sa.transform.position))
                sa.Play();
        }
    }

    private bool CanPlay()
    {
        float rdm = Random.Range(0, 1f);

        if (rdm > 1f * PlayRate)
        {
            return true;
        }

        return false;
    }

    private bool CheckIsNear(Vector3 selfPosition)
    {
        if (Vector3.Distance(m_Player.position, selfPosition) < m_NearDistance)
        {
            return true;
        }

        return false;
    }


    public void Clear()
    {
        m_AnimationGroup.Clear();
        m_Animations.Clear();
        m_GroupIds.Clear();
    }

}
