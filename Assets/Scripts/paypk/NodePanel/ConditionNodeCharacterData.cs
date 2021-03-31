using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConditionNodeCharacterData : MonoBehaviour
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
        NodeData.GetMyNodeData(ref ID, transform).NodeCharactersPanel.SetActive(false);
        NodeData.GetMyNodeData(ref ID, transform).ConditionPanel.GetComponent<ConditionPanel>().AddPanel(Name);
        NodeData.GetMyNodeData(ref ID, transform).ConditionPanel.SetActive(true);
    }
}
