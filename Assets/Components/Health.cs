using UnityEngine;
using System.Collections.Generic;

public class Health : MonoBehaviour
{
    public IntRange hitPoints { get { return _hitPoints; } }
    IntRange _hitPoints;

    public int defaultHP;

    private void Awake()
    {
        _hitPoints = new IntRange(defaultHP);
    }

    public void SetMaxHP(int newMax)
    {
        _hitPoints.second = newMax;
        if (hitPoints.first > hitPoints.second) _hitPoints.first = hitPoints.second;
    }
}