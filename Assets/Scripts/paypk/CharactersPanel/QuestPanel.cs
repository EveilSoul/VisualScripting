using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class QuestPanel: MonoBehaviour
{
    public GameObject Panel;
    public Transform Parent;

    private List<GameObject> quest_Values = new List<GameObject>();

    public void Add()
    {
        var panel = Instantiate(Panel, Parent);
        panel.GetComponent<QuestValue>().Index = quest_Values.Count;
        panel.GetComponent<QuestValue>().Quest_Panel = this;
        quest_Values.Add(panel);
        SetDynamicSize();
    }

    private void SetDynamicSize()
    {
        var transformParent = Parent.GetComponent<RectTransform>();
        var cellSize = Parent.GetComponent<GridLayoutGroup>().cellSize.y;
        transformParent.sizeDelta = new Vector2(transformParent.sizeDelta.x, Math.Max(446, quest_Values.Count * cellSize));
        transformParent.anchoredPosition = new Vector2(0, -transformParent.sizeDelta.y / 2);
    }

    public void Remove(int index)
    {
        // Список квестов в DataManager старый, в quest_Values новый
        // Меняем все отличающиеся имена в DataManager на новые
        if (DataManager.instance.Quests.Count > 0)
        {
            for (int i=0; i < DataManager.instance.Quests.Count; i++)
            {
                var newVal = quest_Values[i].GetComponent<QuestValue>().Name.text;
                var prevVal = DataManager.instance.Quests[i];
                if (!newVal.Equals(prevVal))
                {
                    DataManager.instance.Nodes.ForEach(x =>
                    {
                        if (x.Quest.Name == prevVal)
                            x.Quest = new Quest()
                            {
                                Name = newVal,
                                Type = x.Quest.Type
                            };
                    });
                }
            }
        }

        // Для квеста, который будет удален обнуляем все его значения
        var toDel = quest_Values[index].GetComponent<QuestValue>().Name.text;
        DataManager.instance.Nodes.ForEach(x =>
        {
            if (x.Quest.Name == toDel)
                x.Quest = new Quest();
        });

        // Для всех нод проверяем не был ли выбран данный квест, если да, сбрасываем значение на 0 (или не выбранно)
        foreach(var e in GraphController.NodeData)
        {
            var c = e.Value.ChooseQuestPanel;
            if (c.QuestNameDropdown.value == index + 1)
            {
                c.QuestNameDropdown.value = 0;
            }
        }


        Destroy(quest_Values[index]);
        quest_Values.RemoveAt(index);
        for (int i = index; i < quest_Values.Count; i++)
        {
            quest_Values[i].GetComponent<QuestValue>().Index--;
        }
    }

    public void Save()
    {
        var quests = new List<string>();
        foreach (var e in quest_Values)
        {
            quests.Add(e.GetComponent<QuestValue>().Name.text);
        }

        DataManager.instance.Quests = quests;
    }
}

