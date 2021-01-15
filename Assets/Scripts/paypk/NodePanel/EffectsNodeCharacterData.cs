using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectsNodeCharacterData : MonoBehaviour
{
    public Text NameText;

    public string Name { get; set; }

    public void SetName(string name)
    {
        Name = name;
        NameText.text = name;
    }

    public void OnClick()
    {
        NodeData.instance.EffectsNodeCharactersPanel.SetActive(false);
        NodeData.instance.EffectsPanel.GetComponent<EffectsPanel>().AddPanel(Name);
        NodeData.instance.EffectsPanel.SetActive(true);
    }
}
