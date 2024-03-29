﻿using UnityEngine;
using System.Collections.Generic;

public class EnemyController : Controller
{
    public Collider2D fieldOfVision;
    public enum Type { PREDATOR, PREY, SCAVENGER };
    public Type type;
    public AudioClip awareSFX;
    public AudioClip footstepSFX;

    public enum BehaviorMode { PATROL, CHASE, RUN, ATTACK }
    BehaviorMode curBehavior = BehaviorMode.PATROL;

    float timeToBehaviorCheck = 0f;

    Vector3 curTarget;
    Vector3 homeSpot;

    AudioSource audio;
    float timeToFootstep = 0f;

    private void Start()
    {
        homeSpot = transform.position;
        audio = GetComponent<AudioSource>();
    }
    public override Vector2 GetJoystickDirection()
    {
        return curTarget - transform.position;
    }

    private void Update()
    {
        if (PauseScreen.instance.isVisible) return;
        timeToBehaviorCheck -= Time.deltaTime;
        if(timeToBehaviorCheck <= 0f)
        {
            timeToBehaviorCheck = Random.Range(1f, 2f);
            switch(curBehavior)
            {
                case BehaviorMode.PATROL:
                    if (Vector3.Distance(transform.position, homeSpot) > 10f)
                        curTarget = homeSpot;
                    else if (fieldOfVision.IsTouching(GameEngine.instance.player.GetComponent<Collider2D>()))
                    {
                        if (type == Type.PREY)
                        {
                            curTarget = GameEngine.instance.player.transform.position - transform.position;
                            curTarget *= -10f;
                            curTarget = transform.position + curTarget;
                            GetComponent<EnemyCreature>().StartRunning();
                            curBehavior = BehaviorMode.RUN;
                            audio.clip = awareSFX;
                            audio.Play();
                        }
                        else
                        {
                            curTarget = GameEngine.instance.player.transform.position;
                            curBehavior = BehaviorMode.CHASE;
                            audio.clip = awareSFX;
                            audio.Play();
                        }
                    }
                    
                    else
                        curTarget = transform.position + new Vector3(Random.Range(-6f, 6f), Random.Range(-6f, 6f), 0f);
                break;
                case BehaviorMode.RUN:
                    {
                        if (!fieldOfVision.IsTouching(GameEngine.instance.player.GetComponent<Collider2D>()))
                        {
                            curBehavior = BehaviorMode.PATROL;
                            GetComponent<EnemyCreature>().EndRunning();
                        }
                        else
                        {
                            curTarget = GameEngine.instance.player.transform.position - transform.position;
                            curTarget *= -10f;
                            curTarget = transform.position + curTarget;
                        }
                        break;
                    }
                case BehaviorMode.CHASE:
                    if(Vector3.Distance(transform.position, curTarget) > 10f)
                     {
                        curTarget = homeSpot;
                        curBehavior = BehaviorMode.PATROL;
                     } else if(Vector3.Distance(transform.position, curTarget) < GetComponent<EnemyCreature>().attackRange)
                    { 
                        curBehavior = BehaviorMode.ATTACK;
                        curTarget = GameEngine.instance.player.transform.position - transform.position;
                        curTarget *= 100f;
                        curTarget = transform.position + curTarget;
                        onButtonDown.Invoke(Command.ATTACK);
                        
                        timeToBehaviorCheck = GetComponent<EnemyCreature>().attackCharge + GetComponent<EnemyCreature>().attackTime + .2f;
                    }
                    break;
                case BehaviorMode.ATTACK:
                    curBehavior = BehaviorMode.CHASE;
                    break;
            }
        } else if(curBehavior == BehaviorMode.CHASE)
            {
                curTarget = GameEngine.instance.player.transform.position;
            }
        timeToFootstep -= Time.deltaTime;
        if(timeToFootstep <= 0f)
        {
            if(!audio.isPlaying && footstepSFX != null)
            {
                audio.clip = footstepSFX;
                audio.Play();
            }
            timeToFootstep = .25f;
        }
    }
   
}
