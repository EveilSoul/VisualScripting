using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectsNodeCharacterData : MonoBehaviour
{
    public int ID = -1;

    public Text NameText;

    public string Name { get; set; }

    public void SetName(string name)
    {
        Name = name;
        NameText.text = name;
    }

    public void OnClick()
    {
        NodeData.GetMyNodeData(ref ID, transform).EffectsNodeCharactersPanel.SetActive(false);
        NodeData.GetMyNodeData(ref ID, transform).EffectsPanel.GetComponent<EffectsPanel>().AddPanel(Name);
        NodeData.GetMyNodeData(ref ID, transform).EffectsPanel.SetActive(true);
    }
}
