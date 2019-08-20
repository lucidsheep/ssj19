using UnityEngine;
using DG.Tweening;
using TMPro;
using Rewired;
using UnityEngine.SceneManagement;
public class GameEngine : MonoBehaviour
{
    public static GameEngine instance;

    public PlayerCharacter player;
    public EvolutionMenu evoMenuTemplate;
    public SpriteRenderer fadeToBlack;
    public TextMeshPro titleText;

    public float startingTime;
    public float timeIncrement;

    public int foodRequirement;
    public int foodRequirementIncrement;

    public Biome currentBiome;
    [System.Serializable]
    public class WeightedBiome : LSWeightedItem<Biome> { }
    public WeightedBiome[] biomeOptions;

    public TimeTickEvent onTimeTick = new TimeTickEvent();
    public FoodGatheredEvent onFoodGathered = new FoodGatheredEvent();
    public EvolutionEvent onEvolutionObtained = new EvolutionEvent();
    public EvolutionEvent onEvolutionLost = new EvolutionEvent();
    public MatingPhaseStart onMatingPhase = new MatingPhaseStart();
    public GatheringPhaseStart onGatheringPhase = new GatheringPhaseStart();
    public BiomeChangeEvent onBiomeChanged = new BiomeChangeEvent();

    float timeLeft;
    int foodGathered;
    bool gameOver = false;

    LSWeightedList<Biome> biomeList = new LSWeightedList<Biome>();
    public bool inGatheringPhase = false;

    private void Awake()
    {
        instance = this;
        foreach (var biome in biomeOptions) biomeList.Add(biome);
    }

    private void Start()
    {
        PickBiome();
        TimeControl.StartTimer(.1f, () =>
            StartGathering());
        player.GetComponent<Health>().onHPDepleted.AddListener(EndGathering);
    }
    void StartGathering()
    {
        if (foodGathered > 0) titleText.SetText("A Natural Selection...");
        foodGathered = 0;
        timeLeft = startingTime;
        onTimeTick.Invoke(timeLeft);
        onFoodGathered.Invoke(foodGathered, foodRequirement);
        player.GetComponent<Health>().RestoreHealth(999);
        MapManager.instance.GenerateMap(50, 10);
        fadeToBlack.DOColor(new Color(0f, 0f, 0f, 0f), 1f).SetDelay(1f).SetEase(Ease.Linear).OnComplete(() =>
        {
            inGatheringPhase = true;
            onGatheringPhase.Invoke();
            titleText.SetText("");
        });
        
    }

    private void Update()
    {
        if (gameOver && ReInput.players.GetPlayer(0).GetButtonDown("Start"))
            SceneManager.LoadScene(0);
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

    void PickBiome()
    {
        currentBiome = biomeList.GetRandomItem();
        //todo - prophecy
        MapManager.instance.ProcessBiome(currentBiome);
        onBiomeChanged.Invoke(currentBiome);
    }

    void EndGathering()
    {
        inGatheringPhase = false;
        PickBiome();
        fadeToBlack.DOColor(new Color(0f, 0f, 0f, 1f), 1f).SetEase(Ease.Linear).OnComplete(() =>
        {
            if (foodGathered >= foodRequirement && player.GetComponent<Health>().hitPoints.first > 0)
            {
                Instantiate(evoMenuTemplate, Camera.main.gameObject.transform);
                onMatingPhase.Invoke();
                titleText.SetText("Choose Your Mate");
            }
            else
            {
                titleText.SetText("The Journey Ends...");
                gameOver = true;
            }
            player.transform.position = new Vector3(25f, 25f, 0f);
        });
        
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