using UnityEngine;
using System.Collections;
using TMPro;
using DG.Tweening;
using Rewired;

public class PauseScreen : MonoBehaviour
{

    public TextMeshPro statsTxt;
    public TextMeshPro traitsTxt;
    public SpriteRenderer bg;
    public GameObject mainObject;
    public TextMeshPro titleTxt;

    public bool isVisible = false;

    public static PauseScreen instance;

    bool transitioning = false;

    protected void Awake()
    {
        instance = this;
    }

    public void ShowScreen()
    {
        if (transitioning) return;
        transitioning = isVisible = true;
        SetData();
        bg.DOColor(new Color(0f, 0f, 0f, .5f), .5f).SetUpdate(true);
        mainObject.transform.DOLocalMoveY(0f, .5f).SetEase(Ease.OutBack).SetUpdate(true).OnComplete(() => transitioning = false);
    }

    void SetData()
    {
        titleTxt.SetText("Generation " + GameEngine.instance.numGenerations);
        var player = GameEngine.instance.player;
        var hp = player.GetComponent<Health>();
        var sp = player.GetComponent<Stamina>();
        statsTxt.text = hp.hitPoints.first + "/" + hp.hitPoints.second +
            "\n" + sp.stamina.first + "/" + sp.stamina.second +
            "\n" + player.strength +
            "\n" + player.agility;
        string traits = "";
        foreach(Trait trait in player.traitList)
        {
            traits += "| " + trait.abilityName + "\n";
        }
        traitsTxt.text = traits;
    }

    public void HideScreen()
    {
        if (transitioning || !isVisible) return;
        transitioning = true;
        bg.DOColor(new Color(0f, 0f, 0f, 0f), .5f).SetUpdate(true);
        mainObject.transform.DOLocalMoveY(-10f, .5f).SetEase(Ease.OutBack).SetUpdate(true).OnComplete(() =>
        {
            isVisible = false;
            transitioning = false;
            Time.timeScale = 1f;
        });
    }

    void Update()
    {
        if(isVisible && ReInput.players.GetPlayer(0).GetButtonDown("START"))
        {
            HideScreen();
        }
    }
}
