using UnityEngine;
using System.Collections.Generic;
using Rewired;
using DG.Tweening;
using TMPro;

public class EvolutionMenu : MonoBehaviour
{
    public EvolutionMenuView menuViewTemplate;
    public GameObject selector;
    public TextMeshPro actionTxt;
    public TextMeshPro traitTxt;

    List<EvolutionMenuView> menuItems = new List<EvolutionMenuView>();
    int curSelection = 0;
    Player controller;

    private void Start()
    {
        bool isSexy = GameEngine.instance.player.HasTrait(Trait.Type.ENHANCED_MATING);
        GenerateMenu(isSexy ? 5 : 3);
        controller = ReInput.players.GetPlayer(0);
    }

    void GenerateMenu(int numOptions)
    {
        for(int i = 0; i < numOptions; i++)
        {
            EvolutionMenuView menuItem = Instantiate(menuViewTemplate, this.transform);
            menuItem.transform.localPosition = new Vector3(-4f + (i * 2f), 0f, 1f);
            menuItem.transform.localScale = new Vector3(.75f, .75f, 1f);
            Evolution evolution = new Evolution();
            evolution.GenerateEvolution();
            menuItem.evolution = evolution;
            menuItems.Add(menuItem);
        }
        TimeControl.StartTimer(.1f, () =>
            UpdateSelection());
    }

    void UpdateSelection()
    {
        Evolution curEvolution = menuItems[curSelection].evolution;
        actionTxt.SetText(curEvolution.action == null ? "" : "ACTION: " + curEvolution.action.abilityName + " - " + curEvolution.action.abilityDescription);
        traitTxt.SetText(curEvolution.trait == null ? "" : "TRAIT: " + curEvolution.trait.abilityName + " - " + curEvolution.trait.abilityDescription);
    }
    private void Update()
    {
        if(controller.GetAxis("MoveX") > .5f && controller.GetAxisPrev("MoveX") <= .5f)
        {
            Move(true);
        }
        else if (controller.GetAxis("MoveX") < -.5f && controller.GetAxisPrev("MoveX") >= -.5f)
        {
            Move(false);
        }
        else if(controller.GetButtonDown("Attack"))
        {
            GameEngine.instance.AddEvolution(menuItems[curSelection].evolution);
            Destroy(this.gameObject);
        }
    }

    void Move(bool goingRight)
    {
        bool moved = true;
        if (goingRight && curSelection < menuItems.Count - 1)
        {
            curSelection++;
        }
        else if (!goingRight && curSelection > 0)
        {
            curSelection--;
        }
        else
            moved = false;
        
        if(moved)
        {
            selector.transform.DOKill();
            selector.transform.DOLocalMoveX(-4f + (curSelection * 2f), .25f).SetEase(Ease.OutBack);
            UpdateSelection();
        }
    }
}