using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class EvoCamera : MonoBehaviour {

    public Vector2 deadZone;
    public GameObject target;
    [Range(1, 5)]
    public float cameraSpeed;

    Tweener cameraPan;
    Vector3 lastPosition;

	// Use this for initialization
	void Start () {
	}


    // Update is called once per frame
    void Update () {
        if (target == null) return;
        if (Vector3.Distance(lastPosition, target.transform.position) < .1f) return;

        var pos = Camera.main.WorldToViewportPoint(target.transform.position);
        Vector2 delta = new Vector2();
        if (pos.x > .5f + deadZone.x)
            delta.x = pos.x - deadZone.x - .5f;
        else if (pos.x < .5f - deadZone.x)
            delta.x = pos.x + deadZone.x - .5f;
        if (pos.y > .5f + deadZone.y)
            delta.y = pos.y - deadZone.y - .5f;
        else if (pos.y < .5f - deadZone.y)
            delta.y = pos.y + deadZone.y - .5f;
        if (cameraPan != null)
            cameraPan.Kill();
        lastPosition = target.transform.position;
        delta *= 10f;
        cameraPan = transform.DOMove(delta, .5f / cameraSpeed).SetRelative();
	}
}
