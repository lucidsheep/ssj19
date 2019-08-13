using UnityEngine;
using System.Collections.Generic;

public class Controller : MonoBehaviour
{
    public enum Command { ACTION_A, ACTION_B, ACTION_C, ACTION_D, START, BACK, DASH, ATTACK }

    public OnButtonDown onButtonDown = new OnButtonDown();

    public virtual Vector2 GetJoystickDirection() { return new Vector2(); }
}