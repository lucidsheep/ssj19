using UnityEngine;
using Rewired;

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
}