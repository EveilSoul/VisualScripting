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

    public void Save()
    {
        var propsToAdd = panels.Select(x => x.GetComponent<EffectsPanelCharacterProperty>()).Where(x => (x.Name != "" && x.Value != "" && x.Value != null));
        if (Name != "")
        {
            if (inChange)
            {
                var character = DataManager.instance.Characters.Where(x => x.Name == prevName).First();
                character.Name = Name;
                character.Properties = propsToAdd.Select(x => new CharacterProperty() { Name = x.Name, Type = x.PropertyType, Value = x.Value }).ToList();
                inChange = false;
            }
            else
            {
                var character = new Character();
                character.Name = Name;
                character.Properties = propsToAdd.Select(x => new CharacterProperty() { Name = x.Name, Type = x.PropertyType, Value = x.Value }).ToList();
                DataManager.instance.Characters.Add(character);
            }

        }

    }

    //public GameObject Panel;
    //public Transform Parent;

    //public Text TextName;
    //public string Name { get; set; }

    //private List<GameObject> panels;
    //private List<string> propertiesName;

    //private void SetName(string name)
    //{
    //    TextName.text = name;
    //    Name = name;
    //    propertiesName = DataManager.instance.Characters.First(x => x.Name == name).Properties.Select(x => x.Name).ToList();
    //}

    //public void ClearPanels()
    //{
    //    foreach (var p in panels)
    //    {
    //        Destroy(p);
    //    }
    //    panels = new List<GameObject>();
    //}

    //public void AddPanel(string name)
    //{
    //    var panel = Instantiate(Panel, Parent);
    //    panels.Add(panel);
    //    panel.GetComponent<ConditionPanel>().SetName(name);
    //}

    //public List<string> GetMyProperties(string name)
    //{
    //    return propertiesName.Except(panels.Select(x => x.GetComponent<ConditionPanelCharacterProiperty>()).Select(x => x.Name).Where(x => x != name)).ToList();
    //}
}
