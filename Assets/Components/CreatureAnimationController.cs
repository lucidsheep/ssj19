using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureAnimationController : MonoBehaviour {

    public Controller controller;

    Animator anim;

    bool wasMoving = false;
    bool wasMovingRight = false;
    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    private void Update()
    {
        var move = controller.GetJoystickDirection();
        bool isMoving = move != Vector2.zero;
        if(isMoving != wasMoving)
        {
            wasMoving = isMoving;
            anim.SetBool("IsMoving", wasMoving);

        }
        
        if (move.x > .1f && !wasMovingRight)
        {
            wasMovingRight = true;
            anim.SetBool("IsMovingRight", true);
        }

        else if (move.x < -.1f && wasMovingRight)
        {
            wasMovingRight = false;
            anim.SetBool("IsMovingRight", false);
        }
    }


}
