using UnityEngine;
using System.Collections.Generic;

public class Health : MonoBehaviour
{
    public IntRange hitPoints { get; private set; }

    public int defaultHP;

    private void Awake()
    {
        hitPoints = new IntRange(defaultHP);
    }
}