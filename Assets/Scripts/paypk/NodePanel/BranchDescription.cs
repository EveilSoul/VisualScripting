using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BranchDescription : MonoBehaviour
{
    public int ID = -1;
    public Dropdown TypeDropdown;
    public InputField Text;
    [HideInInspector] public Button Button;


    public void Initialize(int id, BranchType branchType, string text, GameObject node)
    {
        ID = id;
        TypeDropdown.value = (int)branchType;
        Text.text = text;

        var button = node.GetComponentInChildren<Button>();

        if (Text.text.Length > 0)
            QuestButtonColors.ApplyColorsToButton(true, button);
        else
            QuestButtonColors.ApplyColorsToButton(false, button);
    }

    public void Save()
    {

        if (ID == -1)
        {
            NodeData.GetMyNodeData(ref ID, transform);
        }
        if (Text.text.Length > 0)
            QuestButtonColors.ApplyColorsToButton(true, Button);
        else
            QuestButtonColors.ApplyColorsToButton(false, Button);

        BranchManager.SaveBranch(ID, (BranchType)TypeDropdown.value, Text.text);

        gameObject.transform.parent.GetComponentsInChildren<NodeDescriptionCharactersPanel>(true).First(x => x.name == "NodeDescriptionn-Panel").gameObject.SetActive(true);
        gameObject.transform.parent.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
}
