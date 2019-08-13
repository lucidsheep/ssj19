﻿using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Controller))]
[RequireComponent(typeof(Rigidbody2D))]
public class Creature : MonoBehaviour
{
    protected int strength;
    protected int agility;

    public int defaultStrength;
    public int defaultAgility;
    public float defaultSpeedMultiplier = .7f;

    protected Health health;
    protected Controller controller;
    protected Rigidbody2D body;
    protected float speedMultiplier;

    protected virtual void Awake()
    {
        strength = defaultStrength;
        agility = defaultAgility;
        health = GetComponent<Health>();
        controller = GetComponent<Controller>();
        body = GetComponent<Rigidbody2D>();
        speedMultiplier = defaultSpeedMultiplier;
        controller.onButtonDown.AddListener(OnButtonDown);
    }

    protected virtual void Update()
    {
        Vector2 direction = controller.GetJoystickDirection();

        body.MovePosition(body.position + (direction * Time.deltaTime * agility * speedMultiplier));
    }

    protected virtual void OnButtonDown(Controller.Command command)
    {

    }


}