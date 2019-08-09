using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTile : MonoBehaviour {

	public enum TileCategory { TREE, WATER, THORNS, ROCK, FIRE }
	public bool isObstacle;

	public float containsFoodProbability;
	public Food containedFood;

	public bool isDangerous;
	public IntRange damageRange;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
