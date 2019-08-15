using UnityEngine;
using System.Collections;
using DG.Tweening;

public class EnemyCreature : Creature
{
    public float attackSpeedMultiplier;
    public float attackTime;
    public GameObject foodOnDeath;
    public IntRange foodDroppedRange;

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

    protected override void OnDeath()
    {
        base.OnDeath();
        //todo - turn into corpse
        int foodToDrop = foodDroppedRange.second == 0 ? 0 : Random.Range(foodDroppedRange.first, foodDroppedRange.second + 1);
        while(foodToDrop > 0)
        {
            var food = Instantiate(foodOnDeath, transform.position, Quaternion.identity);
            food.GetComponent<Collider2D>().enabled = false;
            food.transform.DOMove(new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), 0f), .2f)
                .SetEase(Ease.InQuad)
                .SetRelative()
                .OnComplete(() => food.GetComponent<Collider2D>().enabled = true);
            foodToDrop--;
        }
        Destroy(this.gameObject);
    }
}
