using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CharactersPanel : MonoBehaviour
{
    public GameObject Panel;
    public GameObject Parent;

    private List<GameObject> panels = new List<GameObject>();

    public void AddPanel(string name)
    {
        var panel = Instantiate(Panel, gameObject.transform);
        panels.Add(panel);
        panel.GetComponent<CharactersProperty>().SetName(name);
    }

    public void UpdatePanels()
    {
        foreach (var p in panels)
        {
            Destroy(p);
        }

        panels = new List<GameObject>();

        foreach (var p in DataManager.instance.Characters)
        {
            AddPanel(p.Name);
        }
        SetDynamicSize();
    }

    private void SetDynamicSize()
    {
        var transformParent = Parent.GetComponent<RectTransform>();
        var cellSize = Parent.GetComponent<GridLayoutGroup>().cellSize.y;
        int count = DataManager.instance.Characters.Count;
        var r = (count / 7 + count % 7 - 1) * (cellSize + 40) + 25;
        transformParent.sizeDelta = new Vector2(transformParent.sizeDelta.x, Math.Max(340, r));
        transformParent.anchoredPosition = new Vector2(0, -transformParent.sizeDelta.y / 2);
    }

    public void UpdatePanelsByNode(List<Character> worldState)
    {
        foreach (var p in panels)
        {
            Destroy(p);
        }

        panels = new List<GameObject>();

        foreach (var p in worldState)
        {
            AddPanel(p.Name);
        }
    }

    public void Delete(string name)
    {
        var panelToDel = panels.First(x => x.GetComponent<CharactersProperty>().Name == name);
        Destroy(panelToDel);
        panels.Remove(panelToDel);
        DataManager.instance.Characters.Remove(DataManager.instance.Characters.First(x => x.Name == name));
    }
}
