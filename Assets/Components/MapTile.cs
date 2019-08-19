using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MapTile : MonoBehaviour {

	public enum TileCategory { TREE, WATER, THORNS, ROCK, FIRE, FOOD, GROUND, CHASM, BUSH, MUSHROOM }
    public TileCategory tileType;
    public int adjacencyBonus;

	public bool isObstacle;

	public float containsFoodProbability;
	public MapTile containedFood;
    public SpriteRenderer sprite;
    public SpriteRenderer foodSpriteRenderer;

    public bool isDangerous;
	public IntRange damageRange;
    public Health.DamageType damageType;

    Interactable interactable;
    bool doesContainFood;


	// Use this for initialization
	void Start () {
        interactable = GetComponent<Interactable>();

        if (containsFoodProbability > Random.value)
        {
            interactable.canInteract = true;
            doesContainFood = true;
            if (tileType != TileCategory.WATER) sprite.enabled = false;
        } else if(foodSpriteRenderer != null)
        {
            Destroy(foodSpriteRenderer.gameObject);
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

            if (tileType == TileCategory.MUSHROOM)
                GameEngine.instance.player.GetComponent<Health>().ReceiveDamage(Random.Range(5, 15), Health.DamageType.POISON);
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
            collision.gameObject.GetComponent<Health>().ReceiveDamage(Random.Range(damageRange.first, damageRange.second), damageType);
        }
    }
}
