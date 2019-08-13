using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTile : MonoBehaviour {

	public enum TileCategory { TREE, WATER, THORNS, ROCK, FIRE, FOOD, GROUND }
    public TileCategory tileType;
	public bool isObstacle;

	public float containsFoodProbability;
	public Food containedFood;

	public bool isDangerous;
	public IntRange damageRange;

    public Sprite[] spriteVariations;

	// Use this for initialization
	void Start () {
        if (spriteVariations.Length > 0)
            GetComponentInChildren<SpriteRenderer>().sprite = spriteVariations[Random.Range(0, spriteVariations.Length)];
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
