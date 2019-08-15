﻿using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerCharacter : Creature
{
	public float defaultDashTime;
	public float defaultDashSpeed;

    public TextMeshPro interactTxt;
    public Attack attackTemplate;

	protected Stamina stamina;

    public List<Evolution> evolutionList = new List<Evolution>();
    public List<Action> actionList = new List<Action>();
    public List<Trait> traitList = new List<Trait>();

    Interactable curInteractTarget = null;

	bool isDashing = false;
	float dashLength;
	float dashSpeed;
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
    }

    void UpdateInteractText()
    {
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
                if(curInteractTarget != null && curInteractTarget.canInteract)
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
        attackAnim = Instantiate(attackTemplate, this.transform);
        attackAnim.transform.localPosition = Vector3.zero;
        Vector2 dir = controller.GetJoystickDirection();
        dir.x *= -1f;
        attackAnim.transform.localRotation = Quaternion.Euler(0f, 0f, Util.Vector2ToAngle(dir) + 90f);
        attackAnim.attackDamage = strength;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Interactable iTarget = collision.gameObject.GetComponent<Interactable>();
        if(curInteractTarget == null && iTarget != null && iTarget.canInteract)
        {
            curInteractTarget = iTarget;
            UpdateInteractText();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        Interactable iTarget = collision.gameObject.GetComponent<Interactable>();
        if(curInteractTarget != null && iTarget != null && curInteractTarget.Equals(iTarget))
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
                traitList.Add(evolution.trait);
            }
        }
    }
}