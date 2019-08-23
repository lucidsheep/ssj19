using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rain : MonoBehaviour {

    public SpriteRenderer rainTemplate;
    List<SpriteRenderer> allRain;
    AudioSource audio;
    Biome lastBiome;
	// Use this for initialization
	void Start () {
        allRain = new List<SpriteRenderer>();
        audio = GetComponent<AudioSource>();
        Util.DoubleLoop(20, 20, (x, y) =>
        {
            var thisRain = Instantiate(rainTemplate, transform);
            thisRain.transform.localPosition = new Vector3(x * 4f, y * 4f, 0f);
            allRain.Add(thisRain);
        });
        GameEngine.instance.onBiomeChanged.AddListener(OnBiomeChanged);
        GameEngine.instance.onGatheringPhase.AddListener(OnGatheringStart);
	}

    void OnGatheringStart()
    {
        Util.Maybe(lastBiome, biome =>
        {
            if (biome.type == Biome.Type.WETLANDS)
                audio.Play();
            else
                audio.Stop();
        });
    }
	
    void OnBiomeChanged(Biome biome)
    {
        lastBiome = biome;
        foreach (var rain in allRain)
            rain.enabled = biome.type == Biome.Type.WETLANDS;
        audio.Stop();
    }

}
