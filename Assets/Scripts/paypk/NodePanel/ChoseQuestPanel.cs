using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ChoseQuestPanel: MonoBehaviour
{
    public int ID = -1;

    public Dropdown QuestNameDropdown;
    public Dropdown QuestTypeDropdown;

    public List<string> options = new List<string>();

    public void OnEnable()
    {
        var first = DataManager.NotSelectedValue;
        options = new List<string> { first }.Union(DataManager.instance.Quests).ToList();
        QuestNameDropdown.options = options.Select(x => new Dropdown.OptionData(x)).ToList();

        if (ID >= 0)
        {
            var chooseQuestPanel = NodeData.instance.GetMyNodeData(ref ID, transform).ChooseQuestPanel;
            QuestNameDropdown.value = chooseQuestPanel.QuestNameDropdown.value;
            QuestTypeDropdown.value = chooseQuestPanel.QuestTypeDropdown.value;
        }
        else
        {
            QuestTypeDropdown.gameObject.SetActive(false);
        }
    }

    public void OnValueChange()
    {
        if (QuestNameDropdown.value > 0)
        {
            QuestTypeDropdown.gameObject.SetActive(true);
        }
        else
        {
            QuestTypeDropdown.gameObject.SetActive(false);
        }
    }

    public void Save()
    {
        NodeData.instance.GetMyNodeData(ref ID, transform);
    }
}

public enum QuestType
{
    Start,
    End
}

