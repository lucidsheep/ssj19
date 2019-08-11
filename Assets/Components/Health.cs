using UnityEngine;
using System.Collections.Generic;

public class Health : MonoBehaviour
{
    public IntRange hitPoints { get { return _hitPoints; } }

    public enum DamageType { PHYSICAL, FIRE, ICE, POISON };
    public enum DamageResilience { NORMAL, WEAK, RESIST, IMMUNE, ABSORB, REFLECT }

    public HPChangeEvent onHPChange = new HPChangeEvent();
    public HPUpdateEvent onHPMaxChange = new HPUpdateEvent();
    public class DamageTypeReslience : LSTuple<DamageType, DamageResilience> { }

    List<DamageTypeReslience> damageRelationships = new List<DamageTypeReslience>();

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

        onHPMaxChange.Invoke(hitPoints);
    }

    public void ReceiveDamage(int damage, DamageType type)
    {
        float multiplier = 1f;

        var relationship = CheckVulnerability(type);
        switch(relationship)
        {
            case DamageResilience.ABSORB: multiplier = -1f; break;

            case DamageResilience.REFLECT:
            case DamageResilience.IMMUNE: multiplier = 0f; break;

            case DamageResilience.RESIST: multiplier = .5f; break;
            case DamageResilience.WEAK: multiplier = 2f; break;

            default: break;
        }
        int totalDamage = Mathf.CeilToInt(damage * multiplier) * -1;
        _hitPoints.first += totalDamage;
        onHPChange.Invoke(totalDamage);
    }
    public DamageResilience CheckVulnerability(DamageType type)
    {
        DamageResilience ret = DamageResilience.NORMAL;
        var relationship = damageRelationships.FindAll(x => x.first == type);
        if (relationship == null) return DamageResilience.NORMAL;
        foreach (var relItem in relationship)
        {
            ret = GetResiliencePriority(ret, relItem.second);
        }
        return ret;
    }

    DamageResilience GetResiliencePriority(DamageResilience a, DamageResilience b)
    {
        if (a == DamageResilience.REFLECT || b == DamageResilience.REFLECT) return DamageResilience.REFLECT;
        if (a == DamageResilience.ABSORB || b == DamageResilience.ABSORB) return DamageResilience.ABSORB;
        if (a == DamageResilience.IMMUNE || b == DamageResilience.IMMUNE) return DamageResilience.IMMUNE;
        if (a == DamageResilience.RESIST || b == DamageResilience.RESIST) return DamageResilience.RESIST;
        if (a == DamageResilience.WEAK || b == DamageResilience.WEAK) return DamageResilience.WEAK;
        return DamageResilience.NORMAL;
    }
}