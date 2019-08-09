using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour {

	public static MapManager instance;

	[System.Serializable]
	public class WeightedMapTile : LSWeightedItem<MapTile> { public WeightedMapTile() : base() { } }

	public MapTile groundTile;
	public WeightedMapTile[] obstacleOptions;

	public IntRange tileSize;

	LSWeightedList<MapTile> mapTiles;

	void Awake()
	{
		instance = this;
		mapTiles = new LSWeightedList<MapTile>();
		foreach(var tile in obstacleOptions)
		{
			mapTiles.Add(tile.item, tile.weight);
		}
	}
	// Use this for initialization
	void Start () {
		GenerateMap(50, 10);
	}

	void GenerateMap(int size, int difficulty)
	{
		Util.DoubleLoop(size, size, (x, y) =>
		{
			MapTile baseTile = Instantiate(groundTile, new Vector3(x * tileSize.x, y * tileSize.y, 0f), Quaternion.identity);

			MapTile randomTile = mapTiles.GetRandomItem();
			if(randomTile != null)
				Instantiate(randomTile, new Vector3(x * tileSize.x, y * tileSize.y, 0f), Quaternion.identity);
		});
	}
	// Update is called once per frame
	void Update () {
		
	}
}
