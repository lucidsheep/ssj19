using UnityEngine;
using TMPro;

public class UITextDisplay : MonoBehaviour
{
    public TextMeshPro text;
    public SpriteRenderer icon;
    public enum Type { FOOD, TIME }
    public Type type;

    string storedText;
    bool shouldDisplay = true;

    private void Start()
    {
        if (type == Type.FOOD)
            GameEngine.instance.onFoodGathered.AddListener(OnFoodGathered);
        else if (type == Type.TIME)
            GameEngine.instance.onTimeTick.AddListener(OnTimeTick);

        GameEngine.instance.onMatingPhase.AddListener(HideDisplay);
        GameEngine.instance.onGatheringPhase.AddListener(ShowDisplay);

    }

    void OnFoodGathered(int gathered, int total)
    {
        SetText(gathered + "/" + total);
    }

    void OnTimeTick(float time)
    {
        SetText(Util.FormatTime(time));
    }

    void SetText(string txt)
    {
        storedText = txt;
        if (!shouldDisplay) return;

        text.SetText(storedText);
    }
    public void HideDisplay()
    {
        shouldDisplay = false;
        icon.enabled = false;
        text.SetText("");
    }
    public void ShowDisplay()
    {
        shouldDisplay = true;
        icon.enabled = true;
        text.SetText(storedText);
    }

}