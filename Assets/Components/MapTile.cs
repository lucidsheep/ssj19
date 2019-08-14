using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MapTile : MonoBehaviour {

	public enum TileCategory { TREE, WATER, THORNS, ROCK, FIRE, FOOD, GROUND }
    public TileCategory tileType;
	public bool isObstacle;

	public float containsFoodProbability;
	public MapTile containedFood;
    public Sprite foodSprite;

	public bool isDangerous;
	public IntRange damageRange;

    public Sprite[] spriteVariations;

    Interactable interactable;
    bool doesContainFood;
    SpriteRenderer foodSpriteRenderer;
    SpriteRenderer sprite;

	// Use this for initialization
	void Start () {
        interactable = GetComponent<Interactable>();
        sprite = GetComponentInChildren<SpriteRenderer>();

        if (spriteVariations.Length > 0)
            sprite.sprite = spriteVariations[Random.Range(0, spriteVariations.Length)];

        if (containsFoodProbability > Random.value)
        {
            interactable.canInteract = true;
            doesContainFood = true;

            foodSpriteRenderer = new GameObject().AddComponent<SpriteRenderer>();
            foodSpriteRenderer.transform.parent = this.transform;
            foodSpriteRenderer.sortingLayerName = "Obstacle_Upper";
            foodSpriteRenderer.transform.localPosition = new Vector3(0f, 0f, 1f);
            foodSpriteRenderer.sprite = foodSprite;

            if (tileType == TileCategory.TREE)
            {
                sprite.sprite = spriteVariations[3]; //hack to force a certain tree type
                sprite.enabled = false;
            }
        }
        
        interactable.onInteraction.AddListener(OnInteraction);
	}

    void OnInteraction(Creature instigator)
    {
        if (doesContainFood)
        {
            Transform t = foodSpriteRenderer != null ? foodSpriteRenderer.transform : sprite.transform;
            DOTween.Sequence()
                .Append(t.DOLocalMoveX(.1f, .05f).SetEase(Ease.Linear).SetRelative())
                .Append(t.DOLocalMoveX(-.2f, .05f).SetEase(Ease.Linear).SetRelative())
                .Append(t.DOLocalMoveX(.2f, .05f).SetEase(Ease.Linear).SetRelative())
                .Append(t.DOLocalMoveX(-.1f, .05f).SetEase(Ease.Linear).SetRelative())
                .AppendCallback(() =>
                {
                    MapTile food = Instantiate(containedFood, transform.position, Quaternion.identity);
                    food.GetComponent<Collider2D>().enabled = false;
                    food.transform.DOMove(instigator.transform.position, .25f).SetEase(Ease.InQuad).OnComplete(() => food.GetComponent<Collider2D>().enabled = true);
                    if (foodSpriteRenderer != null) Destroy(foodSpriteRenderer.gameObject);
                    sprite.enabled = true;
                });
            interactable.canInteract = false;
            doesContainFood = false;
        }
    }
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(tileType == TileCategory.FOOD)
        {
            GameEngine.instance.OnFoodGathered();
            Destroy(this.gameObject);
        }
        else if(isDangerous && collision.gameObject.GetComponent<Health>() != null)
        {
            collision.gameObject.GetComponent<Health>().ReceiveDamage(Random.Range(damageRange.first, damageRange.second), Health.DamageType.FIRE);
        }
    }
}
