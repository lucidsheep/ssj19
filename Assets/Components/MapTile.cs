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

	public bool isDangerous;
	public IntRange damageRange;

    public Sprite[] spriteVariations;

    Interactable interactable;
    bool doesContainFood;

	// Use this for initialization
	void Start () {
        interactable = GetComponent<Interactable>();
        if(containsFoodProbability > Random.value)
        {
            interactable.canInteract = true;
            doesContainFood = true;
            GetComponentInChildren<SpriteRenderer>().color = Color.blue;
        }
        if (spriteVariations.Length > 0)
            GetComponentInChildren<SpriteRenderer>().sprite = spriteVariations[Random.Range(0, spriteVariations.Length)];
        interactable.onInteraction.AddListener(OnInteraction);
	}

    void OnInteraction(Creature instigator)
    {
        if (doesContainFood)
        {
            Transform t = GetComponentInChildren<SpriteRenderer>().transform;
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
                });
            interactable.canInteract = false;
            doesContainFood = false;
            GetComponentInChildren<SpriteRenderer>().color = Color.white;
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
