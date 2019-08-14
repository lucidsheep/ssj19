using UnityEngine;
using Rewired;
using System;

public class HumanController : Controller
{
    Player source;

    private void Awake()
    {
        source = ReInput.players.GetPlayer(0);
    }
    public override Vector2 GetJoystickDirection()
    {
        return source.GetAxis2D("MoveX", "MoveY");
    }

    private void Update()
    {
        if (source.GetButtonDown("DASH")) onButtonDown.Invoke(Command.DASH);
        if (source.GetButtonDown("Attack")) onButtonDown.Invoke(Command.ATTACK);
       
    }
}