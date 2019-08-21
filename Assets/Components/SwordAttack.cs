using UnityEngine;
using System.Collections;
using DG.Tweening;

public class SwordAttack : Attack
{
    public override bool StartAttack()
    {
        if (!base.StartAttack()) return false;

        //transform.DOLocalRotate(new Vector3(0f, 0f, -90f), attackTime).SetRelative().OnComplete(FinishAttack);
        TimeControl.StartTimer(attackTime, FinishAttack);
        return true;
    }

    void Update()
    {

    }
}
