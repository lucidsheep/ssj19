﻿using UnityEngine;
using System.Collections.Generic;

public class EnemyController : Controller
{
    public Collider2D fieldOfVision;
    public enum Type { PREDATOR, PREY, SCAVENGER };
    public Type type;

    public enum BehaviorMode { PATROL, CHASE, RUN, ATTACK }
    BehaviorMode curBehavior = BehaviorMode.PATROL;

    float timeToBehaviorCheck = 0f;

    Vector3 curTarget;
    Vector3 homeSpot;

    private void Start()
    {
        homeSpot = transform.position;
    }
    public override Vector2 GetJoystickDirection()
    {
        return curTarget - transform.position;
    }

    private void Update()
    {
        timeToBehaviorCheck -= Time.deltaTime;
        if(timeToBehaviorCheck <= 0f)
        {
            timeToBehaviorCheck = Random.Range(2f, 4f);
            switch(curBehavior)
            {
                case BehaviorMode.PATROL:
                    if(fieldOfVision.IsTouching(GameEngine.instance.player.GetComponent<Collider2D>()))
                    {
                        curTarget = GameEngine.instance.player.transform.position;
                        curBehavior = BehaviorMode.CHASE;
                    }
                    else if(Vector3.Distance(transform.position, homeSpot) > 10f)
                        curTarget = homeSpot;
                    else
                        curTarget = transform.position + new Vector3(Random.Range(-6f, 6f), Random.Range(-6f, 6f), 0f);
                break;
                case BehaviorMode.CHASE:
                    if(Vector3.Distance(transform.position, curTarget) > 10f)
                     {
                        curTarget = homeSpot;
                        curBehavior = BehaviorMode.PATROL;
                     } else if(Vector3.Distance(transform.position, curTarget) < 2f)
                    { 
                        curBehavior = BehaviorMode.ATTACK;
                        curTarget = GameEngine.instance.player.transform.position;
                        onButtonDown.Invoke(Command.ATTACK);
                        //timeToBehaviorCheck = .75f;
                    }
                    break;
                case BehaviorMode.ATTACK:
                    curBehavior = BehaviorMode.CHASE;
                    break;
            }
        } else if(curBehavior == BehaviorMode.CHASE || curBehavior == BehaviorMode.ATTACK)
            {
                curTarget = GameEngine.instance.player.transform.position;
            }

        

    }
}