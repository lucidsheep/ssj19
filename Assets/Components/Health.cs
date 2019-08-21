using UnityEngine;
using System.Collections.Generic;

public class Health : MonoBehaviour
{
    public IntRange hitPoints { get { return new IntRange(Mathf.FloorToInt(actualHitPoints), _hitPoints.second); } }
    public SpriteBlinker blinkAnim;
    public enum DamageType { PHYSICAL, FIRE, ICE, POISON, NORMAL };
    public enum DamageResilience { NORMAL, WEAK, RESIST, IMMUNE, ABSORB, REFLECT }

    public HPChangeEvent onHPChange = new HPChangeEvent();
    public HPUpdateEvent onHPMaxChange = new HPUpdateEvent();
    public HPDepletedEvent onHPDepleted = new HPDepletedEvent();
    public InvincibilityChangeEvent onInvincibilityChange = new InvincibilityChangeEvent();

    public class DamageTypeReslience : LSTuple<DamageType, DamageResilience> {
        public DamageTypeReslience(DamageType t, DamageResilience r)
        {
            first = t;
            second = r;
        }
    }

    List<DamageTypeReslience> damageRelationships = new List<DamageTypeReslience>();

    IntRange _hitPoints;
    public int defaultHP;
    public float defaultInvincibleTime;

    
    public float defaultRecoveryPerSecond = 5;
    public float defaultTimeToStartRecovery = 5f;

    float _recoveryRate = 5f;
    float _recoveryStartupTime;

    float actualHitPoints;

    float recoveryCooldown;
    float invincibleTime = 0f;
    private void Awake()
    {
        _hitPoints = new IntRange(defaultHP);
        actualHitPoints = _hitPoints.first;
        _recoveryRate = defaultRecoveryPerSecond;
        _recoveryStartupTime = defaultTimeToStartRecovery;
    }

    private void Update()
    {
        if (invincibleTime >= 0f)
        {
            invincibleTime -= Time.deltaTime;
            if (invincibleTime <= 0f)
                onInvincibilityChange.Invoke(false);
        }
        if (recoveryCooldown > 0f)
            recoveryCooldown -= Time.deltaTime;
        else if (actualHitPoints < _hitPoints.second)
        {
            float delta = Time.deltaTime * _recoveryRate;
            int before = hitPoints.first;
            actualHitPoints = actualHitPoints + delta >= hitPoints.second ? hitPoints.second : actualHitPoints + delta;

            int intDelta = hitPoints.first - before;
            if (intDelta > 0)
                onHPChange.Invoke(intDelta);
        }
    }
    public void SetMaxHP(int newMax)
    {
        _hitPoints.second = newMax;
        if (actualHitPoints > hitPoints.second) actualHitPoints = hitPoints.second;

        onHPMaxChange.Invoke(hitPoints);
    }

    public void SetRecoveryRate(float newRate)
    {
        _recoveryRate = newRate;
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
        if (invincibleTime > 0f)
            return;
        List<Attack> attacks = new List<Attack>();
        foreach (Attack atk in source.GetComponents<Attack>()) attacks.Add(atk);
        foreach (Attack atk in source.GetComponentsInParent<Attack>()) attacks.Add(atk);
        if (attacks.Count == 0) return;
        var attack = attacks[0];
        if (attack != null && attack.isAttacking && source.tag == "Dangerous")
            ReceiveDamage(attack.attackDamage, attack.attackType);
    }
    public void ReceiveDamage(int damage, DamageType type, bool ignoreInvincibility = false)
    {
        if (invincibleTime > 0f && !ignoreInvincibility) return;

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

        if (totalDamage >= 0) return;

        actualHitPoints += totalDamage;
        onHPChange.Invoke(totalDamage);
        recoveryCooldown = _recoveryStartupTime;
        if (actualHitPoints <= 0f)
            onHPDepleted.Invoke();
        else if(!ignoreInvincibility)
        {
            invincibleTime = defaultInvincibleTime;
            onInvincibilityChange.Invoke(true);
            if (blinkAnim != null)
                blinkAnim.StartAnim(invincibleTime);
        }
    }

    public void RestoreHealth(int amount)
    {
        float before = actualHitPoints;
        actualHitPoints = Mathf.Min(actualHitPoints + amount, _hitPoints.second);
        onHPChange.Invoke(Mathf.FloorToInt(actualHitPoints - before));
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

    public void AddResilience(DamageType damage, DamageResilience newRes)
    {
        var curRes = CheckVulnerability(damage);
        if (curRes != newRes && GetResiliencePriority(curRes, newRes) == newRes)
            damageRelationships.Add(new DamageTypeReslience(damage, newRes));
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