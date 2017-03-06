using UnityEngine;
using System.Collections;
using System;

public class RPGMapCombat : MonoBehaviour
{
    private Action battleCallBack;

    private Action standingEventCallBack;

    private float meetEnemyTime;

    private float meetEnemyTimeLong;

    private float meetEnemyProbability;

    private float standingEventProbability;

    private float standingEventTimeLong;

    private float standingEventTime;

    private int checkEvent = 0;

    private bool isMoving;

    public void EnterMap(float _meetEnemyProbability, float _meetEnemyTimeLong, float _standingEventProbability, float _standingEventTimeLong)
    {

        meetEnemyProbability = _meetEnemyProbability;

        meetEnemyTimeLong = _meetEnemyTimeLong;

        standingEventProbability = _standingEventProbability;

        standingEventTimeLong = _standingEventTimeLong;

        meetEnemyTime = Time.time + meetEnemyTimeLong;

        standingEventTime = Time.time + standingEventTimeLong;
    }

    public void SetCallBack(Action _battleCallBack, Action _standingEventCallBack)
    {
        battleCallBack = _battleCallBack;

        standingEventCallBack = _standingEventCallBack;
    }

    public void StartCheckEvent()
    {
        checkEvent++;

        if (checkEvent > 1)
        {

            throw new Exception("StartCheckMeetEnemy error!");
        }

        if (checkEvent > 0)
        {

            meetEnemyTime = Time.time + meetEnemyTimeLong;
        }
    }

    public void StopCheckEvent()
    {
        checkEvent--;
    }

    public void SetMoveState(bool _isMoving)
    {
        isMoving = _isMoving;
    }

    public void FixMeetEnemyProbability(float _fix)
    {
        meetEnemyProbability += _fix;
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving)
        {
            standingEventTime = Time.time + standingEventTimeLong;
        }

        if (checkEvent > 0)
        {
            if (!isMoving && Time.time > standingEventTime)
            {
                standingEventTime = Time.time + standingEventTimeLong;

                if (UnityEngine.Random.value < standingEventProbability)
                {

                    if (standingEventCallBack != null)
                    {

                        standingEventCallBack();
                    }
                }
            }
            else if (Time.time > meetEnemyTime)
            {

                meetEnemyTime = Time.time + meetEnemyTimeLong;

                if (UnityEngine.Random.value < meetEnemyProbability)
                {

                    if (battleCallBack != null)
                    {

                        battleCallBack();
                    }
                }
            }
        }
    }
}
