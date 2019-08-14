using UnityEngine;
using System.Collections;
using System;

public class Attack : MonoBehaviour
{
    public Collider2D hurtBox;

    public float attackTime = .1f;
    public int attackDamage = 1;
    public Health.DamageType attackType = Health.DamageType.NORMAL;
    public bool autoAttack;
    public bool destroyOnAttackFinish;

    public bool isAttacking;

    virtual protected void Start()
    {
        if(autoAttack)
            StartAttack();

    }

    virtual public bool StartAttack()
    {
        if (isAttacking) return false;
        isAttacking = true;
        return true;
    }

    virtual public void FinishAttack()
    {
        isAttacking = false;
        if (destroyOnAttackFinish)
            Destroy(this.gameObject);
    }
}
