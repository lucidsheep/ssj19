using UnityEngine;

[CreateAssetMenu()]
public class Trait : Ability
{
    public enum Type
    {
        IMMUNITY_FIRE,
        IMMUNITY_ICE,
        ENHANCED_DASH,
        SWIMMING,
        IMMUNITY_POISON,
        HUNTER,
        ENHANCED_REGEN,
        ENHANCED_MATING,
        INTERACT_TREE,
        INTERACT_FISH,
        ENHANCED_VISION
    }

    public Type type;

}