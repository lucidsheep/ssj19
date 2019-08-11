using UnityEngine;

public class GameEngine : MonoBehaviour
{
    public static GameEngine instance;

    public PlayerCharacter player;
    public EvolutionMenu evoMenuTemplate;

    public float startingTime;
    public float timeIncrement;

    public int foodRequirement;
    public int foodRequirementIncrement;

    public TimeTickEvent onTimeTick = new TimeTickEvent();
    public FoodGatheredEvent onFoodGathered = new FoodGatheredEvent();
    public EvolutionEvent onEvolutionObtained = new EvolutionEvent();
    public EvolutionEvent onEvolutionLost = new EvolutionEvent();
    public MatingPhaseStart onMatingPhase = new MatingPhaseStart();
    public GatheringPhaseStart onGatheringPhase = new GatheringPhaseStart();

    float timeLeft;
    int foodGathered;
    bool inGatheringPhase = false;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        TimeControl.StartTimer(.1f, () =>
            StartGathering());
    }
    void StartGathering()
    {
        foodGathered = 0;
        timeLeft = startingTime;
        onTimeTick.Invoke(timeLeft);
        onFoodGathered.Invoke(foodGathered, foodRequirement);
        inGatheringPhase = true;
        onGatheringPhase.Invoke();
    }

    private void Update()
    {
        if (!inGatheringPhase) return;
        int timeBefore = Mathf.CeilToInt(timeLeft);
        timeLeft -= Time.deltaTime;
        if (timeBefore != Mathf.CeilToInt(timeLeft))
        {
            onTimeTick.Invoke(timeLeft);
            if (timeLeft <= 0f)
                EndGathering();
        }
    }

    void EndGathering()
    {
        inGatheringPhase = false;
        if(foodGathered >= foodRequirement)
        {
            Instantiate(evoMenuTemplate, Camera.main.gameObject.transform);
            onMatingPhase.Invoke();
        }
    }
    public void AddEvolution(Evolution evolution)
    {
        player.AddEvolution(evolution);

        foodRequirement += foodRequirementIncrement;
        startingTime += timeIncrement;
        StartGathering();
    }

    public void OnFoodGathered()
    {
        foodGathered++;
        onFoodGathered.Invoke(foodGathered, foodRequirement);
    }
}