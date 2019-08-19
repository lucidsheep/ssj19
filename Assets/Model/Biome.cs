using UnityEngine;
using System.Collections;

[CreateAssetMenu()]
public class Biome : ScriptableObject
{
    [System.Serializable]
    public enum Type { NORMAL, WETLANDS, DROUGHT, EARTHQUAKE, FROZEN}
    public Type type;
    public string biomeName;
    public string biomeProphecy;

    [System.Serializable]
    public class WeightedMapTile : LSWeightedItem<MapTile> { }

    public WeightedMapTile[] tileAdjustments;

}
