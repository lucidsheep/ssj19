using System.Collections;
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
    public bool shouldShake;

    IntRange storedValues;
    Tweener ratioTween;
    float ratio { get { return _ratio; } set { _ratio = value; bar.SetFill(_ratio); } }
    float _ratio = 1f;
    bool shouldDisplay = true;

    float shakeTime = 0f;
    Vector3 basePosition;
    float shakeAmount = .05f;

    private void Start()
    { 
        GameEngine.instance.onMatingPhase.AddListener(HideDisplay);
        GameEngine.instance.onGatheringPhase.AddListener(ShowDisplay);
        GameEngine.instance.onBiomeChanged.AddListener(biome => HideDisplay());

        storedValues = type == Type.HEALTH ? target.GetComponent<Health>().hitPoints : target.GetComponent<Stamina>().stamina;

        if(type == Type.HEALTH)
        {
            target.GetComponent<Health>().onHPChange.AddListener(OnChange);
            target.GetComponent<Health>().onHPMaxChange.AddListener(OnMaxChange);
        }
        else if(type == Type.STAMINA)
        {
            target.GetComponent<Stamina>().onSPChange.AddListener(OnChange);
            target.GetComponent<Stamina>().onSPMaxChange.AddListener(OnMaxChange);
        }
        
    }

    public void OnChange(int delta)
    {
        storedValues.first += delta;
        if (!shouldDisplay) return;
        AnimateBar(Mathf.Abs(delta) <= 5);
        if(shouldShake && delta < -2)
            Shake(.25f);
    }

    void Shake(float time)
    {
        if (shakeTime <= 0f)
        {
            basePosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
            Util.Maybe(GetComponent<EdgeAnchor>(), anchor => anchor.enabled = false);
        }
        shakeTime = time;
    }
    public void OnMaxChange(IntRange newRange)
    {
        storedValues = newRange;
        if(shouldDisplay)
            AnimateBar(true);
    }

    void AnimateBar(bool instant = false)
    {
        if (ratioTween != null) ratioTween.Kill();
        if (instant) ratio = (float)storedValues.first / (float)storedValues.second;
        else
            ratioTween = DOTween.To(() => ratio, x => ratio = x, (float)storedValues.first / (float)storedValues.second, .25f).SetEase(Ease.InOutExpo);

    }
    public void ShowDisplay()
    {
        shouldDisplay = true;
        icon.enabled = bg.enabled = true;
        ratio = (float)storedValues.first / (float)storedValues.second;
    }

    public void HideDisplay()
    {
        shouldDisplay = false;
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
            {
                transform.localPosition = basePosition;
                Util.Maybe(GetComponent<EdgeAnchor>(), anchor => anchor.enabled = true);
            }
            else
                transform.localPosition = new Vector3(basePosition.x + Random.Range(-shakeAmount, shakeAmount), basePosition.y + Random.Range(-shakeAmount, shakeAmount), basePosition.z);
        }

    }

}
