using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NodeCharacterData : MonoBehaviour
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
        NodeData.instance.NodeCharactersPanel.SetActive(false);
        NodeData.instance.ConditionPanel.GetComponent<ConditionPanel>().AddPanel(Name);
        NodeData.instance.ConditionPanel.SetActive(true);
    }
}
