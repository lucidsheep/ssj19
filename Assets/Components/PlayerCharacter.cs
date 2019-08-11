using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : Creature
{
    IntRange staminaPoints;
    public int startingSP;
    public Evolution[] evolutionList = new Evolution[5];

    protected override void Awake()
    {
        base.Awake();
        staminaPoints = new IntRange(startingSP, startingSP);
    }

    private void Start()
    {
        for(int i = 0; i < 5; i++)
        {
            evolutionList[i] = new Evolution();
            evolutionList[i].GenerateEvolution();
        }
    }
}