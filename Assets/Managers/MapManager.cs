using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour {

	public static MapManager instance;

	[System.Serializable]
	public class WeightedMapTile : LSWeightedItem<GameObject> { public WeightedMapTile() : base() { } }

	public WeightedMapTile[] obstacleOptions;
	LSWeightedList<GameObject> mapTiles;

	void Awake()
	{
		instance = this;
		//mapTiles = new LSWeightedList<GameObject>{obstacleOptions};
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
