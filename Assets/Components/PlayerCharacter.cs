using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerCharacter : Creature
{
	public float defaultDashTime;
	public float defaultDashSpeed;
    public float defaultSwimSpeed;

    public TextMeshPro interactTxt;
    public Attack attackTemplate;
    public Anim dashAnimTemplate;
    public Evolution debug_forcedEvolution;

	protected Stamina stamina;

    public List<Evolution> evolutionList = new List<Evolution>();
    public List<Action> actionList = new List<Action>();
    public List<Trait> traitList = new List<Trait>();

    Interactable curInteractTarget = null;

	bool isDashing = false;
	float dashLength;
	float dashSpeed;
    int swimTimeoutFrames = 0;
    float swimDuration = 0f;
    float swimTick = .1f;
    Attack attackAnim;
    Vector2 lastMovementVector;

    protected override void Awake()
    {
        base.Awake();
		dashLength = defaultDashTime;
		dashSpeed = defaultDashSpeed;
    }

    private void Start()
    {
		stamina = GetComponent<Stamina>();
        Util.Maybe(debug_forcedEvolution, evo =>
        {
            AddEvolution(evo);
        });
        GameEngine.instance.onBiomeChanged.AddListener(OnBiomeChanged);
    }

    void OnBiomeChanged(Biome newBiome)
    {
        if(newBiome.type == Biome.Type.FROZEN && !HasTrait(Trait.Type.IMMUNITY_ICE))
        {
            health.SetRecoveryRate(1f);
            stamina.SetRecoveryRate(stamina.defaultRecoveryPerSecond / 2f);
        } else
        {
            health.SetRecoveryRate(HasTrait(Trait.Type.ENHANCED_REGEN) ? 5f : health.defaultRecoveryPerSecond);
            stamina.SetRecoveryRate(stamina.defaultRecoveryPerSecond);
        }
    }
    override protected void Update()
    {
        base.Update();
        if(controller.GetJoystickDirection() != Vector2.zero)
            lastMovementVector = controller.GetJoystickDirection();
        if (swimTimeoutFrames > 0)
        {
            if(controller.GetJoystickDirection() != Vector2.zero)
                swimTimeoutFrames--;
            speedMultiplier = defaultSwimSpeed;
            swimDuration += Time.deltaTime;
            while(swimDuration > swimTick)
            {
                swimDuration -= swimTick;
                if(!stamina.ConsumeSP(1))
                {
                    health.ReceiveDamage(1, Health.DamageType.NORMAL, true);
                }
            }
        }
        else if (swimTimeoutFrames == 0)
        {
            swimTimeoutFrames--;
            speedMultiplier = defaultSpeedMultiplier;
            swimDuration = 0f;
        }
    }

    public void UpdateInteractText(string customText = "")
    {
        if(customText != "")
        {
            interactTxt.SetText(customText);
            return;
        }
        if (curInteractTarget == null) interactTxt.SetText("");
        else
        {
            interactTxt.SetText("<sprite name=\"" + "button_A" + "\">: " + curInteractTarget.interactText);
        }
    }
    protected override void OnButtonDown(Controller.Command command)
    {
        switch (command)
        {
            
            case Controller.Command.DASH:
                if (!isDashing) StartDash();
                break;
            case Controller.Command.ATTACK:
                if(curInteractTarget != null && curInteractTarget.canInteract && !MapManager.instance.CheckForThreats(transform.position))
                {
                    curInteractTarget.StartInteraction(this);
                    if(!curInteractTarget.canInteract)
                    {
                        curInteractTarget = null;
                        UpdateInteractText();
                    }
                }
                else
                {
                    StartAttack();
                }
                break;
        }
    }

    void StartAttack()
    {
        if (attackAnim != null) return;
        if (!stamina.ConsumeSP(10)) return;
        attackAnim = Instantiate(attackTemplate, this.transform);
        attackAnim.transform.localPosition = Vector3.zero;
        Vector2 dir = lastMovementVector;
        dir.x *= -1f;
        attackAnim.transform.localRotation = Quaternion.Euler(0f, 0f, Util.Vector2ToAngle(dir) + 90f);
        attackAnim.attackDamage = strength;
        attackAnim.transform.localScale = Vector3.one * (HasTrait(Trait.Type.ENHANCED_ATTACK) ? 2.7f : 1.8f);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        CheckInteractionEnter(collision.gameObject);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        CheckInteractionEnter(collider.gameObject);
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        CheckInteractionExit(collision.gameObject);
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        CheckInteractionExit(collider.gameObject);
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        if(HasTrait(Trait.Type.SWIMMING))
        {
            Util.Maybe(collider.gameObject.GetComponent<MapTile>(), tile =>
            {
                if (tile.tileType == MapTile.TileCategory.WATER)
                    swimTimeoutFrames = 3;
            });
        }
    }
    void CheckInteractionEnter(GameObject obj)
    {
        Interactable iTarget = obj.GetComponent<Interactable>();
        if (iTarget != null && iTarget.canInteract)
        {
            curInteractTarget = iTarget;
            UpdateInteractText();
        }
    }

    void CheckInteractionExit(GameObject obj)
    {
        Interactable iTarget = obj.GetComponent<Interactable>();
        if (curInteractTarget != null && iTarget != null && curInteractTarget.Equals(iTarget))
        {
            curInteractTarget = null;
            UpdateInteractText();
        }
    }
    void StartDash()
	{
        if (!stamina.ConsumeSP(20)) return;

		speedMultiplier = dashSpeed;
		isDashing = true;
        Instantiate(dashAnimTemplate, transform.position, Quaternion.identity);
		TimeControl.StartTimer(dashLength, () =>
		{
			speedMultiplier = defaultSpeedMultiplier;
			isDashing = false;
		});
	}

    public void AddEvolution(Evolution evolution)
    {
        evolutionList.Add(evolution);
        ProcessEvolution(evolution);
    }

    void ProcessEvolution(Evolution evolution)
    {
        health.SetMaxHP(health.hitPoints.second + evolution.hp);
        stamina.SetMaxSP(stamina.stamina.second + evolution.sp);
        strength += evolution.str;
        agility += evolution.agi;

        if(evolution.action != null)
        {
            if (actionList.Find(x => x.id == evolution.action.id) == null) //check for duplicate action
            {
                actionList.Add(evolution.action);
            }
        }
        if(evolution.trait != null)
        {
            if(traitList.Find(x => x.id == evolution.trait.id) == null)
            {
                ProcessTrait(evolution.trait);
            }
        }
    }


    void ProcessTrait(Trait trait)
    {
        traitList.Add(trait);
        switch (trait.type)
        {
            case Trait.Type.IMMUNITY_FIRE:
                GetComponent<Health>().AddResilience(Health.DamageType.FIRE, Health.DamageResilience.IMMUNE);
                break;
            case Trait.Type.IMMUNITY_ICE:
                GetComponent<Health>().AddResilience(Health.DamageType.ICE, Health.DamageResilience.IMMUNE);
                break;
            case Trait.Type.ENHANCED_DASH:
                dashSpeed *= 2f;
                dashLength /= 2f;
                break;
            case Trait.Type.IMMUNITY_POISON:
                GetComponent<Health>().AddResilience(Health.DamageType.POISON, Health.DamageResilience.IMMUNE);
                break;
            case Trait.Type.ENHANCED_VISION:
                Camera.main.orthographicSize = 4f;
                break;
            case Trait.Type.ENHANCED_REGEN:
                GetComponent<Health>().SetRecoveryRate(5f);
                break;
        }
    }

    public bool HasTrait(Trait.Type type)
    {
        return traitList.Exists(x => x.type == type);
    }
}