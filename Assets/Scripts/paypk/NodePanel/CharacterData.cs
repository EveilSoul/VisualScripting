using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterData : MonoBehaviour
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
        NodeData.instance.GetMyNodeData(ref ID, transform).CharactersPanel.SetActive(false);
        NodeData.instance.GetMyNodeData(ref ID, transform).NodeDescriptionPanel.GetComponent<NodeDescriptionCharactersPanel>().AddCharacter(Name);
        NodeData.instance.GetMyNodeData(ref ID, transform).NodeDescriptionPanel.SetActive(true);
    }
}
