using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MapManager : MonoBehaviour {

	public static MapManager instance;

	[System.Serializable]
	public class WeightedMapTile : LSWeightedItem<MapTile> { public WeightedMapTile() : base() { } }
    [System.Serializable]
    public class WeightedEnemy : LSWeightedItem<EnemyCreature> { public WeightedEnemy() : base() { } }

	public MapTile groundTile;
	public WeightedMapTile[] obstacleOptions;
    public WeightedEnemy[] enemyOptions;
    public EnemyCreature apexPredator;

    public UnityEvent onMapGenerated = new UnityEvent();

	public IntRange tileSize;

	LSWeightedList<MapTile> mapTiles;
    LSWeightedList<EnemyCreature> enemies;

    List<MapTile> mapTileObjects = new List<MapTile>();
    List<MapTile> mapTileBaseObjects = new List<MapTile>();
    List<EnemyCreature> enemyObjects = new List<EnemyCreature>();


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
		
	}

    public bool CheckForThreats(Vector3 center)
    {
        return enemyObjects.Find(x => x != null && x.gameObject != null && Vector3.Distance(x.transform.position, center) < 2f);
    }
	public void GenerateMap(int size, int difficulty)
	{
        ClearMap();

        LSWeightedList<MapTile> adjacencyMapTiles = new LSWeightedList<MapTile>();
        foreach (LSWeightedItem<MapTile> tile in mapTiles)
            if(tile.item != null)
                adjacencyMapTiles.Add(new LSWeightedItem<MapTile>(tile.item, 0));
        Vector3 midPoint = new Vector3(size / 2f, size / 2f, 0f);
        Util.DoubleLoop(size, size, (x, y) =>
		{
			MapTile baseTile = Instantiate(groundTile, new Vector3(x * tileSize.x, y * tileSize.y, 0f), Quaternion.identity);
            mapTileBaseObjects.Add(baseTile);
            Vector3 pos = new Vector3(x * tileSize.x, y * tileSize.y, 0f);

            if (Vector3.Distance(pos, midPoint) >= 5f)
            {
                foreach (MapTile t in mapTileObjects.FindAll(i => Vector3.Distance(i.transform.position, pos) <= 1.05f))
                {
                    adjacencyMapTiles.Find(i => i.item.tileType == t.tileType).weight += t.adjacencyBonus;
                    mapTiles.Find(i => i.item != null && i.item.tileType == t.tileType).weight += t.adjacencyBonus;
                }
                MapTile randomTile = mapTiles.GetRandomItem();
                foreach (LSWeightedItem<MapTile> t in adjacencyMapTiles.FindAll(i => i.weight > 0))
                {
                    mapTiles.Find(i => i.item != null && i.item.tileType == t.item.tileType).weight -= t.weight;
                    t.weight = 0;
                }
                if (randomTile != null)
                    mapTileObjects.Add(Instantiate(randomTile, pos, Quaternion.identity));
                else
                {
                    Util.Maybe(enemies.GetRandomItem(), randomEnemy =>
                    {
                        enemyObjects.Add(Instantiate(randomEnemy, pos, Quaternion.identity));
                    });
                }
            }
        });
        Vector3 apexLocation = midPoint;
        apexLocation.x *= UnityEngine.Random.value > .5f ? 1.5f : .5f;
        apexLocation.y *= UnityEngine.Random.value > .5f ? 1.5f : .5f;

        enemyObjects.Add(Instantiate(apexPredator, apexLocation, Quaternion.identity));

        onMapGenerated.Invoke();
	}

    internal void ProcessBiome(Biome currentBiome)
    {
        foreach(var tileMod in currentBiome.tileAdjustments)
        {
            Util.Maybe(mapTiles.Find(x => x.item != null && x.item.tileType == tileMod.item.tileType), justTile =>
            {
                justTile.weight = Mathf.Max(0, justTile.weight + tileMod.weight);
            });
        }
    }

    void ClearMap()
    {
        foreach (var tile in mapTileBaseObjects) Destroy(tile.gameObject);
        foreach (var tile in mapTileObjects) Destroy(tile.gameObject);
        foreach (var enemy in enemyObjects) if(enemy != null && enemy.gameObject != null) Destroy(enemy.gameObject);

        mapTileBaseObjects = new List<MapTile>();
        mapTileObjects = new List<MapTile>();
        enemyObjects = new List<EnemyCreature>();
    }
	// Update is called once per frame
	void Update () {
		
	}
}
