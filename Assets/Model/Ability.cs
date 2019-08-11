using UnityEngine;

public class Ability : ScriptableObject
{
    public int id;
    public int value;
    public enum AbilityType { ACTION, TRAIT }
    public AbilityType abilityType;
    public bool isRecessive;
    public string abilityName;
    public string abilityDescription;
}