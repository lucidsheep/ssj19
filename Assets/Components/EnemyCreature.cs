using UnityEngine;
using System.Collections;

public class EnemyCreature : Creature
{
    public float attackSpeedMultiplier;
    public float attackTime;

    override protected void OnButtonDown(Controller.Command command)
	{
        if(command == Controller.Command.ATTACK)
        {
            speedMultiplier = attackSpeedMultiplier;
            GetComponent<Attack>().isAttacking = true;
            TimeControl.StartTimer(attackTime, () => {
                speedMultiplier = defaultSpeedMultiplier;
                GetComponent<Attack>().isAttacking = false;
            });
        }
	}
}
