using UnityEngine;
using System.Collections;
using DG.Tweening;

public class EnemyCreature : Creature
{
    public float attackSpeedMultiplier;
    public float attackTime;
    public GameObject foodOnDeath;
    public IntRange foodDroppedRange;

    LSTimer pounceTimer;
    override protected void OnButtonDown(Controller.Command command)
	{
        if(command == Controller.Command.ATTACK)
        {
            speedMultiplier = 0f;
            float stareTime = Random.Range(.2f, .4f);
            GetComponentInChildren<SpriteRenderer>().DOColor(Color.red, stareTime);
            pounceTimer = TimeControl.StartTimer(stareTime, () => {
                speedMultiplier = attackSpeedMultiplier;
                GetComponent<Attack>().isAttacking = true;
                pounceTimer = TimeControl.StartTimer(attackTime, () =>
                {
                    speedMultiplier = defaultSpeedMultiplier;
                    GetComponent<Attack>().isAttacking = false;
                    GetComponentInChildren<SpriteRenderer>().color = Color.white;
                });
                
            });
        }
	}

    protected override void OnDeath()
    {
        base.OnDeath();
        if (pounceTimer != null) TimeControl.RemoveTimer(pounceTimer.id);
        //todo - turn into corpse
        int foodToDrop = foodDroppedRange.second == 0 ? 0 : Random.Range(foodDroppedRange.first, foodDroppedRange.second + 1);
        if (GameEngine.instance.player.HasTrait(Trait.Type.HUNTER)) foodToDrop *= 2;
        while(foodToDrop > 0)
        {
            var food = Instantiate(foodOnDeath, transform.position, Quaternion.identity);
            food.GetComponent<Collider2D>().enabled = false;
            food.transform.DOMove(new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f), .4f)
                .SetEase(Ease.InQuad)
                .SetRelative()
                .OnComplete(() => food.GetComponent<Collider2D>().enabled = true);
            foodToDrop--;
        }
        Destroy(this.gameObject);
    }
}
