using UnityEngine;

[System.Serializable]
public class Evolution
{
     IntRange hpRange = new IntRange(-10, 10);
    IntRange spRange = new IntRange(-5, 5);
     IntRange strRange = new IntRange(-3, 3);
    IntRange agiRange = new IntRange(-3, 3);

    public Action action;
    public Trait trait;

    public int hp, sp, str, agi;

    public void GenerateEvolution(int minValue = 80, int maxValue = 100)
    {
        int generatedValue = 9999;
        while(generatedValue > maxValue || generatedValue < minValue)
        {
            hp = Random.Range(hpRange.first, hpRange.second + 1);
            sp = Random.Range(spRange.first, spRange.second + 1);
            str = Random.Range(strRange.first, strRange.second + 1);
            agi = Random.Range(agiRange.first, agiRange.second + 1);

            action = Random.value > .5f ? AbilityList.GetRandomAction() : null;
            trait = Random.value > .5f ? AbilityList.GetRandomTrait() : null;

            generatedValue = GetValue();
        }
    }

    public int GetValue()
    {
        int ret = 0;

        ret += hp * 2;
        ret += sp * 2;
        ret += str * 20;
        ret += agi * 20;
        if (action != null) ret += action.value;
        if (trait != null) ret += trait.value;

        return ret;
    }
}