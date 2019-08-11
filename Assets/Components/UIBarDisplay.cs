using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBarDisplay : MonoBehaviour {

    public SpriteRenderer icon;
    public SpriteRenderer bg;
    public LSFillBar bar;

    float storedRatio = 1.0f;

    private void Start()
    {
        GameEngine.instance.onMatingPhase.AddListener(HideDisplay);
        GameEngine.instance.onGatheringPhase.AddListener(ShowDisplay);
    }

    public void ShowDisplay()
    {
        icon.enabled = bg.enabled = true;
        bar.SetFill(storedRatio);
    }

    public void HideDisplay()
    {
        icon.enabled = bg.enabled = false;
        bar.SetFill(0f);
    }

}
