using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : Creature
{
    IntRange staminaPoints;
    public int startingSP;

    protected override void Awake()
    {
        base.Awake();
        staminaPoints = new IntRange(startingSP, startingSP);
    }
}