using UnityEngine;
using System.Collections.Generic;

public class SpriteBiomeVariations : MonoBehaviour
{
    [System.Serializable]
    public class BiomeSprite : LSTuple<Sprite, Biome.Type> { }

    public BiomeSprite[] variations;
    void Start()
    {
        Util.Maybe(GetComponent<SpriteRenderer>(), sr =>
        {
            foreach (var variation in variations)
                if (variation.second == GameEngine.instance.currentBiome.type)
                {
                    sr.sprite = variation.first;
                    break;
                }
        });
    }

}
