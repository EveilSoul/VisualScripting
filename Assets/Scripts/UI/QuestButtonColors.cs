using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestButtonColors : MonoBehaviour
{
    private static QuestButtonColors instance;

    public ButtonColors NoConditionColors;
    public ButtonColors HasConditionColors;

    public static void ApplyColorsToButton(bool hasConditionsNow, Button button)
    {
        var colors = button.colors;
        if (hasConditionsNow)
        {
            colors.normalColor = colors.selectedColor = instance.HasConditionColors.NormalColor;
            colors.highlightedColor = instance.HasConditionColors.HighlightedColor;
            colors.pressedColor = instance.HasConditionColors.PressedColor;
        }
        else
        {
            colors.normalColor = colors.selectedColor = instance.NoConditionColors.NormalColor;
            colors.highlightedColor = instance.NoConditionColors.HighlightedColor;
            colors.pressedColor = instance.NoConditionColors.PressedColor;
        }
        button.colors = colors;
    }

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }
}

[System.Serializable]
public struct ButtonColors
{
    public Color NormalColor;
    public Color HighlightedColor;
    public Color PressedColor;
}
