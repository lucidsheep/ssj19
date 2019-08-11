﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UIBarDisplay : MonoBehaviour {

    public SpriteRenderer icon;
    public SpriteRenderer bg;
    public LSFillBar bar;
    public enum Type { HEALTH, STAMINA }
    public Type type;
    public GameObject target;

    IntRange storedValues;
    Tweener ratioTween;
    float ratio { get { return _ratio; } set { _ratio = value; bar.SetFill(_ratio); } }
    float _ratio = 1f;

    float shakeTime = 0f;
    Vector3 basePosition;
    float shakeAmount = .05f;

    private void Start()
    { 
        GameEngine.instance.onMatingPhase.AddListener(HideDisplay);
        GameEngine.instance.onGatheringPhase.AddListener(ShowDisplay);

        storedValues = type == Type.HEALTH ? target.GetComponent<Health>().hitPoints : new IntRange();

        if(type == Type.HEALTH)
        {
            target.GetComponent<Health>().onHPChange.AddListener(OnChange);
            target.GetComponent<Health>().onHPMaxChange.AddListener(OnMaxChange);
        }

        basePosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
    }

    public void OnChange(int delta)
    {
        storedValues.first += delta;
        AnimateBar();
        Shake(.25f);
    }

    void Shake(float time)
    {
        shakeTime = time;
    }
    public void OnMaxChange(IntRange newRange)
    {
        storedValues = newRange;
        AnimateBar();
    }

    void AnimateBar()
    {
        if (ratioTween != null) ratioTween.Kill();
        ratioTween = DOTween.To(() => ratio, x => ratio = x, (float)storedValues.first / (float)storedValues.second, .25f).SetEase(Ease.InOutExpo);

    }
    public void ShowDisplay()
    {
        icon.enabled = bg.enabled = true;
        bar.SetFill(ratio);
    }

    public void HideDisplay()
    {
        icon.enabled = bg.enabled = false;
        if (ratioTween != null) ratioTween.Kill();
        bar.SetFill(0f);
    }

    private void Update()
    {
        if(shakeTime > 0f)
        {
            shakeTime -= Time.deltaTime;
            if (shakeTime <= 0f)
                transform.localPosition = basePosition;
            else
                transform.localPosition = new Vector3(basePosition.x + Random.Range(-shakeAmount, shakeAmount), basePosition.y + Random.Range(-shakeAmount, shakeAmount), basePosition.z);
        }

    }

}