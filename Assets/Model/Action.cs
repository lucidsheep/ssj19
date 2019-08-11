using UnityEngine;

[CreateAssetMenu()]
public class Action : Ability
{
    public enum Type
    {
        FLY,
        SWIM,
        WEB,
        HIDE
    }
    public Type type;
    public int staminaCost;
    public enum UsageType { PRESS, HOLD, ON_OFF }
    public UsageType usageType;
}