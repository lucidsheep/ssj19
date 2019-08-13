﻿using UnityEngine;
using System.Collections.Generic;

public class Stamina : MonoBehaviour
{
    public IntRange stamina { get { return new IntRange(Mathf.FloorToInt(actualStamina), _stamina.second); } }

    public HPChangeEvent onSPChange = new HPChangeEvent();
    public HPUpdateEvent onSPMaxChange = new HPUpdateEvent();

    IntRange _stamina;
    public int defaultStamina;

    float actualStamina;
    float _recoveryRate = 5f;

    float recoveryCooldown = 0f;

    private void Awake()
    {
        _stamina = new IntRange(defaultStamina);
        actualStamina = defaultStamina;
    }

    public void SetMaxSP(int newMax)
    {
        _stamina.second = newMax;
        if (actualStamina > stamina.second)
        {
            actualStamina = stamina.second;
        }

        onSPMaxChange.Invoke(stamina);
    }

    public bool ConsumeSP(int cost)
    {
        if (cost > actualStamina) return false;

        actualStamina -= cost;
        recoveryCooldown = 3f;
        onSPChange.Invoke(-cost);

        return true;
    }
    private void Update()
    {
        if (recoveryCooldown > 0f)
            recoveryCooldown -= Time.deltaTime;
        else if(actualStamina < _stamina.second)
        {
            float delta = Time.deltaTime * _recoveryRate;
            int before = stamina.first;
            actualStamina = actualStamina + delta >= stamina.second ? stamina.second : actualStamina + delta;

            int intDelta = stamina.first - before;
            if(intDelta > 0)
                onSPChange.Invoke(intDelta);
        }
    }

}