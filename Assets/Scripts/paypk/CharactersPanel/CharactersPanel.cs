using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CharactersPanel : MonoBehaviour
{
    public GameObject Panel;

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
