using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rain : MonoBehaviour {

    public SpriteRenderer rainTemplate;
    List<SpriteRenderer> allRain;
	// Use this for initialization
	void Start () {
        allRain = new List<SpriteRenderer>();
        Util.DoubleLoop(20, 20, (x, y) =>
        {
            var thisRain = Instantiate(rainTemplate, transform);
            thisRain.transform.localPosition = new Vector3(x * 4f, y * 4f, 0f);
            allRain.Add(thisRain);
        });
        GameEngine.instance.onBiomeChanged.AddListener(OnBiomeChanged);
	}
	
    void OnBiomeChanged(Biome biome)
    {
        foreach (var rain in allRain)
            rain.enabled = biome.type == Biome.Type.WETLANDS;
    }

}
