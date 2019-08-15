using UnityEngine;
using System.Collections.Generic;

public class Health : MonoBehaviour
{
    public IntRange hitPoints { get { return _hitPoints; } }

    public enum DamageType { PHYSICAL, FIRE, ICE, POISON, NORMAL };
    public enum DamageResilience { NORMAL, WEAK, RESIST, IMMUNE, ABSORB, REFLECT }

    public HPChangeEvent onHPChange = new HPChangeEvent();
    public HPUpdateEvent onHPMaxChange = new HPUpdateEvent();
    public HPDepletedEvent onHPDepleted = new HPDepletedEvent();
    public InvincibilityChangeEvent onInvincibilityChange = new InvincibilityChangeEvent();

    public class DamageTypeReslience : LSTuple<DamageType, DamageResilience> { }

    List<DamageTypeReslience> damageRelationships = new List<DamageTypeReslience>();

    IntRange _hitPoints;
    public int defaultHP;
    public float defaultInvincibleTime;

    float invincibleTime = 0f;
    private void Awake()
    {
        _hitPoints = new IntRange(defaultHP);
    }

    private void Update()
    {
        if (invincibleTime >= 0f)
        {
            invincibleTime -= Time.deltaTime;
            if (invincibleTime <= 0f)
                onInvincibilityChange.Invoke(false);
        }
    }
    public void SetMaxHP(int newMax)
    {
        _hitPoints.second = newMax;
        if (hitPoints.first > hitPoints.second) _hitPoints.first = hitPoints.second;

        onHPMaxChange.Invoke(hitPoints);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        CheckAndHandleAttack(collision.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CheckAndHandleAttack(collision.gameObject);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        CheckAndHandleAttack(collision.gameObject);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        CheckAndHandleAttack(collision.gameObject);
    }
    void CheckAndHandleAttack(GameObject source)
    {
        if (invincibleTime > 0f) return;
        var attacks = source.GetComponents<Attack>();
        var attack = attacks.Length == 0 ? null : attacks[0];

        if(GetComponent<EnemyCreature>() != null && attack != null)
        {
            Debug.Log("handle attack source " + LayerMask.LayerToName(attack.gameObject.layer));
        }
        if (attack != null && attack.isAttacking)
            ReceiveDamage(attack.attackDamage, attack.attackType);
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
        if (hitPoints.first <= 0)
            onHPDepleted.Invoke();
        else
        {
            invincibleTime = defaultInvincibleTime;
            onInvincibilityChange.Invoke(true);
        }
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