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

    public AudioClip titleMusic;
    public AudioClip[] overworldMusic;
    public AudioClip matingMusic;
    public AudioClip gameOverMusic;
    public AudioClip eatSFX;

    int overworldMusicIndex = 0;

    public float startingTime;
    public float timeIncrement;
    public int mapSize = 50;
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
    public int numGenerations = 1;
    bool gameOver = false;

    LSWeightedList<Biome> biomeList = new LSWeightedList<Biome>();
    public bool inGatheringPhase = false;
    public bool inProphecyPhase = true;

    private void Awake()
    {
        instance = this;
        foreach (var biome in biomeOptions) biomeList.Add(biome);
    }

    private void Start()
    {
        SetBiomeBias();
        PickBiome();
        CenterPlayer();
        player.GetComponent<Health>().onHPDepleted.AddListener(EndGathering);
        MapManager.instance.onMapGenerated.AddListener(OnMapGenerated);
        SoundController.PlaySong(titleMusic);
    }

    void SetBiomeBias()
    {
        Biome commonBiome;
        Biome unCommonBiome;
        do
        {
            commonBiome = biomeList.GetRandomItem();
            unCommonBiome = biomeList.GetRandomItem();
        } while (commonBiome.biomeName == unCommonBiome.biomeName || commonBiome.biomeName == "Grasslands"); //grasslands should never be the common biome

        biomeList.Find(x => x.item.biomeName == commonBiome.biomeName).weight = 30;
        biomeList.Find(x => x.item.biomeName == unCommonBiome.biomeName).weight = 5;
    }
    void StartGathering()
    {
        titleText.SetText("A Natural Selection...\n\n\nLoading World");
        
        TimeControl.StartTimer(.1f, () =>
        {
            foodGathered = 0;
            timeLeft = startingTime;
            onTimeTick.Invoke(timeLeft);
            onFoodGathered.Invoke(foodGathered, foodRequirement);
            player.GetComponent<Health>().RestoreHealth(999);
            MapManager.instance.GenerateMap(mapSize, 10);
        });
    }

    void OnMapGenerated()
    {
        SoundController.FadeOut(.75f);
        fadeToBlack.DOColor(new Color(0f, 0f, 0f, 0f), 1f).SetEase(Ease.Linear).OnComplete(() =>
        {
            inGatheringPhase = true;
            onGatheringPhase.Invoke();
            titleText.SetText("");
            SoundController.PlaySong(overworldMusic[overworldMusicIndex], true, .75f);
            overworldMusicIndex = overworldMusicIndex + 1 >= overworldMusic.Length ? 0 : overworldMusicIndex + 1;
        });
    }
    private void Update()
    {
        if(ReInput.players.GetPlayer(0).GetButtonDown("Start"))
        {
            if (gameOver)
                SceneManager.LoadScene(0);
            else if (inProphecyPhase)
            {
                inProphecyPhase = false;
                StartMating();
            }
            else if(!PauseScreen.instance.isVisible)
            {
                Time.timeScale = 0f;
                PauseScreen.instance.ShowScreen();
            }
        }
        else if (inProphecyPhase && ReInput.players.GetPlayer(0).GetButtonDown("Attack"))
        {
            inProphecyPhase = false;
            StartMating();
        }

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
        titleText.SetText("Generation " + numGenerations + "\n\nThe elders predict " + currentBiome.biomeProphecy + "\n\nPress START to begin natural selection...");
        MapManager.instance.ProcessBiome(currentBiome);
        onBiomeChanged.Invoke(currentBiome);
    }

    void EndGathering()
    {
        inGatheringPhase = false;
        SoundController.FadeOut(.75f);
        
        fadeToBlack.DOColor(new Color(0f, 0f, 0f, 1f), 1f).SetEase(Ease.Linear).OnComplete(() =>
        {
            if (foodGathered >= foodRequirement && player.GetComponent<Health>().hitPoints.first > 0)
            {
                inProphecyPhase = true;
                numGenerations++;
                PickBiome();
                SoundController.PlaySong(matingMusic, true, .75f);
            }
            else
            {
                titleText.SetText("The Journey Ends...\n\nYour species lasted " + numGenerations + " generation" + (numGenerations > 1 ? "s" : "") + ".");
                SoundController.PlaySong(gameOverMusic, true, .75f);
                gameOver = true;
            }
            CenterPlayer();
            MapManager.instance.ClearMap();
        });
        
    }

    void CenterPlayer()
    {
        player.transform.position = new Vector3(mapSize / 2f, mapSize / 2f, 0f);
        Camera.main.transform.position = new Vector3(mapSize / 2f, mapSize / 2f, -10f);
    }

    void StartMating()
    {
        inGatheringPhase = false;
        Instantiate(evoMenuTemplate, Camera.main.gameObject.transform);
        onMatingPhase.Invoke();
        titleText.SetText("Choose Your Mate");
        
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
        SoundController.PlaySFX(eatSFX);
        onFoodGathered.Invoke(foodGathered, foodRequirement);
        if (foodGathered >= foodRequirement)
            EndGathering();
    }
}