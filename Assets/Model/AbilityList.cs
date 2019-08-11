using UnityEngine;

public class AbilityList : MonoBehaviour
{
    public static AbilityList instance;

    public Action[] actionList;
    public Trait[] traitList;

    private void Awake()
    {
        instance = this;
        for(int i = 0; i < actionList.Length; i++)
        {
            actionList[i].id = i;
        }
        for (int i = 0; i < traitList.Length; i++)
        {
            traitList[i].id = i;
        }
    }

    public static Action GetRandomAction()
    {
        return instance.actionList[Random.Range(0, instance.actionList.Length)];
    }

    public static Trait GetRandomTrait()
    {
        return instance.traitList[Random.Range(0, instance.traitList.Length)];
    }


}