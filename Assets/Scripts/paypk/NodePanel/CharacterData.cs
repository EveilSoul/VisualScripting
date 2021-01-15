using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterData : MonoBehaviour
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
        NodeData.instance.CharactersPanel.SetActive(false);
        NodeData.instance.NodeDescriptionPanel.GetComponent<NodeDescriptionCharactersPanel>().AddCharacter(Name);
        NodeData.instance.NodeDescriptionPanel.SetActive(true);
    }
}
