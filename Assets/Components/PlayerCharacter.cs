using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : Creature
{
    IntRange staminaPoints;
    public int startingSP;
    public List<Evolution> evolutionList = new List<Evolution>();
    public List<Action> actionList = new List<Action>();
    public List<Trait> traitList = new List<Trait>();

    protected override void Awake()
    {
        base.Awake();
        staminaPoints = new IntRange(startingSP, startingSP);
    }

    private void Start()
    {

    }

    public void AddEvolution(Evolution evolution)
    {
        evolutionList.Add(evolution);
        ProcessEvolution(evolution);
    }

    void ProcessEvolution(Evolution evolution)
    {
        health.SetMaxHP(health.hitPoints.second + evolution.hp);
        staminaPoints.second += evolution.sp;
        strength += evolution.str;
        agility += evolution.agi;

        if(evolution.action != null)
        {
            if (actionList.Find(x => x.id == evolution.action.id) == null) //check for duplicate action
            {
                actionList.Add(evolution.action);
            }
        }
        if(evolution.trait != null)
        {
            if(traitList.Find(x => x.id == evolution.trait.id) == null)
            {
                traitList.Add(evolution.trait);
            }
        }
    }
}