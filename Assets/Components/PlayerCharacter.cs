using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : Creature
{
	public float defaultDashTime;
	public float defaultDashSpeed;

	protected Stamina stamina;

    public List<Evolution> evolutionList = new List<Evolution>();
    public List<Action> actionList = new List<Action>();
    public List<Trait> traitList = new List<Trait>();

	bool isDashing = false;
	float dashLength;
	float dashSpeed;

    protected override void Awake()
    {
        base.Awake();
		dashLength = defaultDashTime;
		dashSpeed = defaultDashSpeed;
    }

    private void Start()
    {
		stamina = GetComponent<Stamina>();
        controller.onButtonDown.AddListener(OnButtonDown);
    }

    protected override void OnButtonDown(Controller.Command command)
    {
        switch(command)
        {
            case Controller.Command.DASH:
                if (!isDashing) StartDash();
                break;
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