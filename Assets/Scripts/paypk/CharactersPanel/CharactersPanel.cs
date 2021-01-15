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
        panel.GetComponent<CharacterPropert>().SetName(name);
    }

    public void UpdatePanels()
    {
        foreach (var p in panels)
        {
            Destroy(p);
        }

        foreach (var p in DataManager.instance.Characters)
        {
            AddPanel(p.Name);
        }
    }

    public void Delete(string name)
    {
        var panelToDel = panels.Where(x => x.GetComponent<CharacterPropert>().Name == name).First();
        Destroy(panelToDel);
        panels.Remove(panelToDel);
        DataManager.instance.Characters.Remove(DataManager.instance.Characters.First(x => x.Name == name));
    }
}
