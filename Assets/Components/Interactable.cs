using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour {

    public string interactText;
    public bool canInteract;
    public bool disableAfterInteraction;

    public InteractionEvent onInteraction = new InteractionEvent();

    public void StartInteraction(Creature instigator)
    {
        if (!canInteract) return;
        onInteraction.Invoke(instigator);
        if (disableAfterInteraction) canInteract = false;
    }
}
