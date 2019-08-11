using UnityEngine;
using System.Collections.Generic;
public class EvolutionMenu : MonoBehaviour
{
    public EvolutionMenuView menuViewTemplate;

    List<EvolutionMenuView> menuItems = new List<EvolutionMenuView>();
    int curSelection = 0;

    private void Start()
    {
        GenerateMenu(3);
    }

    void GenerateMenu(int numOptions)
    {
        for(int i = 0; i < numOptions; i++)
        {
            EvolutionMenuView menuItem = Instantiate(menuViewTemplate, new Vector3(-4f + (i * 2f), 0f, 0f), Quaternion.identity);
            menuItem.transform.parent = this.transform;
            menuItem.transform.localScale = new Vector3(.75f, .75f, 1f);
            Evolution evolution = new Evolution();
            evolution.GenerateEvolution();
            menuItem.evolution = evolution;
            menuItems.Add(menuItem);
        }
    }
}