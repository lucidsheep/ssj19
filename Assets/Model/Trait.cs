using UnityEngine;

[CreateAssetMenu()]
public class Trait : Ability
{
    public enum Type
    {
        IMMUNITY_FIRE,
        IMMUNITY_ICE,
        ENHANCED_DASH
    }

    public Type type;

}