using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NodeDescriptionCharacter : MonoBehaviour
{
    public Text TextName;
    public string Name { get; set; }

    public void SetName(string name)
    {
        TextName.text = name;
        Name = name;
    }

    public void Delete()
    {
        NodeData.instance.NodeDescriptionPanel.GetComponent<NodeDescriptionCharactersPanel>().Delete(Name);
    }
}
