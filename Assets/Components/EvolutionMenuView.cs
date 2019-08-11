using UnityEngine;
using TMPro;

public class EvolutionMenuView : MonoBehaviour
{
    public TextMeshPro titleTxt, hpTxt, spTxt, strTxt, agiTxt, actionTxt, traitTxt;
    public Evolution evolution;

    private void Start()
    {
        //if (evolution == null) return;
        evolution = new Evolution();
        evolution.GenerateEvolution();

        titleTxt.SetText("Evo Name");
        hpTxt.SetText(AddPlus(evolution.hp));
        spTxt.SetText(AddPlus(evolution.sp));
        strTxt.SetText(AddPlus(evolution.str));
        agiTxt.SetText(AddPlus(evolution.agi));

        if (evolution.action != null)
            actionTxt.SetText(evolution.action.abilityName);
        else
            actionTxt.SetText("");

        if (evolution.trait != null)
            traitTxt.SetText(evolution.trait.abilityName);
        else
            traitTxt.SetText("");
    }

    string AddPlus(int input)
    {
        return (input >= 0 ? "+" : "") + input.ToString();
    }
}