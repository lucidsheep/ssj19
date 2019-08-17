using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour {

	public static MapManager instance;

	[System.Serializable]
	public class WeightedMapTile : LSWeightedItem<MapTile> { public WeightedMapTile() : base() { } }
    [System.Serializable]
    public class WeightedEnemy : LSWeightedItem<EnemyCreature> { public WeightedEnemy() : base() { } }

	public MapTile groundTile;
	public WeightedMapTile[] obstacleOptions;
    public WeightedEnemy[] enemyOptions;

	public IntRange tileSize;

	LSWeightedList<MapTile> mapTiles;
    LSWeightedList<EnemyCreature> enemies;

    List<MapTile> mapTileObjects = new List<MapTile>();


	void Awake()
	{
		instance = this;
		mapTiles = new LSWeightedList<MapTile>();
		foreach(var tile in obstacleOptions)
		{
			mapTiles.Add(tile.item, tile.weight);
		}
        enemies = new LSWeightedList<EnemyCreature>();
        foreach (var enemy in enemyOptions)
            enemies.Add(enemy);
	}
	// Use this for initialization
	void Start () {
		GenerateMap(50, 10);
	}

	void GenerateMap(int size, int difficulty)
	{
        LSWeightedList<MapTile> adjacencyMapTiles = new LSWeightedList<MapTile>();
        foreach (LSWeightedItem<MapTile> tile in mapTiles)
            if(tile.item != null)
                adjacencyMapTiles.Add(new LSWeightedItem<MapTile>(tile.item, 0));

        Util.DoubleLoop(size, size, (x, y) =>
		{
			MapTile baseTile = Instantiate(groundTile, new Vector3(x * tileSize.x, y * tileSize.y, 0f), Quaternion.identity);
            Vector3 pos = new Vector3(x * tileSize.x, y * tileSize.y, 0f);
            foreach(MapTile t in mapTileObjects.FindAll(i => Vector3.Distance(i.transform.position, pos) <= 1.05f))
            {
                adjacencyMapTiles.Find(i => i.item.tileType == t.tileType).weight += t.adjacencyBonus;
                mapTiles.Find(i => i.item != null && i.item.tileType == t.tileType).weight += t.adjacencyBonus;
            }
			MapTile randomTile = mapTiles.GetRandomItem();
            foreach(LSWeightedItem<MapTile> t in adjacencyMapTiles.FindAll(i => i.weight > 0))
            {
                mapTiles.Find(i => i.item != null && i.item.tileType == t.item.tileType).weight -= t.weight;
                t.weight = 0;
            }
			if(randomTile != null)
				mapTileObjects.Add(Instantiate(randomTile, pos, Quaternion.identity));
            else
            {
                var randomEnemy = enemies.GetRandomItem();
                if(randomEnemy != null)
                    Instantiate(randomEnemy, pos, Quaternion.identity);

            }
        });
	}
	// Update is called once per frame
	void Update () {
		
	}
}
