using UnityEngine;
using System.Collections;
using DG.Tweening;

public class EnemyCreature : Creature
{
    public float attackSpeedMultiplier;
    public float attackTime;
    public float attackCharge = .5f;
    public float attackRange = 2f;
    public GameObject foodOnDeath;
    public IntRange foodDroppedRange;
    public AudioClip attackSFX;

    LSTimer pounceTimer;
    bool isAttacking = false;
    bool isRunning = false;
    AudioSource audio;

    protected void Start()
    {
        audio = GetComponent<AudioSource>();
    }
    override protected void OnButtonDown(Controller.Command command)
	{
        if(command == Controller.Command.ATTACK)
        {
            if (isAttacking) return;
            isAttacking = true;
            speedMultiplier = 0f;
            float stareTime = attackCharge;
            GetComponentInChildren<SpriteRenderer>().DOColor(Color.blue, stareTime);
            GetComponent<Animator>().enabled = false;
            pounceTimer = TimeControl.StartTimer(stareTime, () => {
                speedMultiplier = attackSpeedMultiplier;
                GetComponent<Attack>().isAttacking = true;
                audio.clip = attackSFX;
                audio.Play();
                pounceTimer = TimeControl.StartTimer(attackTime, () =>
                {
                    speedMultiplier = defaultSpeedMultiplier;
                    GetComponent<Attack>().isAttacking = false;
                    isAttacking = false;
                    GetComponent<Animator>().enabled = true;
                    GetComponentInChildren<SpriteRenderer>().color = Color.white;
                });
                
            });
        }
	}

    void OnDestroy()
    {
        if (pounceTimer != null) TimeControl.RemoveTimer(pounceTimer.id);
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

    public void StartRunning()
    {
        isRunning = true;
        speedMultiplier = attackSpeedMultiplier;
    }

    public void EndRunning()
    {
        isRunning = false;
        speedMultiplier = defaultSpeedMultiplier;
    }

    protected override void Update()
    {
        base.Update();
        if(isRunning)
        {
            if (Random.value > .75 && !GetComponent<Stamina>().ConsumeSP(1))
                EndRunning();
        }
    }
}
