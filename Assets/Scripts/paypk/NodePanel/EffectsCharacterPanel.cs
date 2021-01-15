using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EffectsCharacterPanel : MonoBehaviour
{
    public GameObject Panel;
    public Transform Parent;
    public Button AddPropertyButton;
    public Text NameText;

    public string Name { get; set; }

    private bool inChange = false;
    private string prevName;
    private List<string> propertiesName;
    public List<GameObject> panels;


    public void SetName(string name)
    {
        panels = new List<GameObject>();
        propertiesName = DataManager.instance.Properties.Keys.ToList();
        NameText.text = name;
        Name = name;
        ClearPanels();
        AddPropertyButton.interactable = true;
        inChange = false;
    }

    public void ClearPanels()
    {
        foreach (var p in panels)
        {
            Destroy(p);
        }
        panels = new List<GameObject>();
    }

    public void OpenExistCharacter(string name, List<PropertyValue> propertyValues)
    {
        inChange = true;


        NameText.text = name;
        Name = name;
        foreach (var p in propertyValues)
        {
            OnAddClick();
        }


        for (int i = 0; i < panels.Count; i++)
        {
            var charAdd = panels[i].GetComponent<EffectsPanelCharacterProperty>();
            var property = propertyValues[i];
            charAdd.SetPropertyName(property.Name);
        }

        for (int i = 0; i < panels.Count; i++)
        {
            var charAdd = panels[i].GetComponent<EffectsPanelCharacterProperty>();
            var property = propertyValues[i];
            charAdd.SetPropertyValue(property.PropertyType, property.Value);
        }
    }

    public void OnChangePropertyType(string Name)
    {
        foreach (var p in panels.Select(x => x.GetComponent<EffectsPanelCharacterProperty>()).Where(x => x.Name != Name))
        {
            p.OnChooseNewProperty();
        }
    }

    public List<string> GetMyProperties(string name)
    {
        if (name == "")
            return propertiesName.Except(panels.Select(x => x.GetComponent<EffectsPanelCharacterProperty>()).Select(x => x.Name)).ToList();
        return propertiesName.Except(panels.Select(x => x.GetComponent<EffectsPanelCharacterProperty>()).Select(x => x.Name).Where(x => x != name)).ToList();
    }

    public void OnAddClick()
    {
        var panel = Instantiate(Panel, Parent);
        panels.Add(panel);
        panel.GetComponent<EffectsPanelCharacterProperty>().Initial(Name);
        if (panels.Count == propertiesName.Count)
            AddPropertyButton.interactable = false;
    }

    public void Delete(string name)
    {
        var panelToDel = panels.Where(x => x.GetComponent<EffectsPanelCharacterProperty>().Name == name).First();
        Destroy(panelToDel);
        panels.Remove(panelToDel);
        AddPropertyButton.interactable = true;
        OnChangePropertyType("");
    }

    public void DeleteMe()
    {
        NodeData.instance.EffectsPanel.GetComponent<EffectsPanel>().Delete(Name);
    }
}
