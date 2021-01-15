using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EffectsPanel : MonoBehaviour
{
    public GameObject Panel;
    public Transform Parent;

    public Button AddButton;
    public Button SaveButton;

    public List<GameObject> panels = new List<GameObject>();
    public List<string> Names = new List<string>();

    public void AddPanel(string name)
    {
        var panel = Instantiate(Panel, Parent);
        panels.Add(panel);
        panel.GetComponent<EffectsCharacterPanel>().SetName(name);
        Names.Add(name);
    }

    public void OpenExist(string name, List<PropertyValue> propertyValues)
    {
        var panel = Instantiate(Panel, Parent);
        panels.Add(panel);
        panel.GetComponent<EffectsCharacterPanel>().SetName(name);
        panel.GetComponent<EffectsCharacterPanel>().OpenExistCharacter(name, propertyValues);
        Names.Add(name);
    }

    public List<string> GetMyProperties(string parentName, string name)
    {
        return panels.Select(x => x.GetComponent<EffectsCharacterPanel>()).First(x => x.Name == parentName).GetMyProperties(name);
    }

    public void OnChangePropertyType(string parentName, string name)
    {
        panels.Select(x => x.GetComponent<EffectsCharacterPanel>()).First(x => x.Name == parentName).OnChangePropertyType(name);
    }

    public void ClearAll()
    {
        foreach (var p in panels)
        {
            Destroy(p);
        }
        panels = new List<GameObject>();
    }

    public void Delete(string name)
    {
        Names.Remove(name);
        var p = panels.First(x => x.GetComponent<EffectsCharacterPanel>().Name == name);
        panels.Remove(p);
        Destroy(p);
    }

    public void DeleteProperty(string parentName, string name)
    {
        panels.Select(x => x.GetComponent<EffectsCharacterPanel>()).First(x => x.Name == parentName).Delete(name);
    }
}
