using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class CharacterAddPanel : MonoBehaviour
{
    public GameObject Panel;
    public Transform Parent;
    public Button AddPropertyButton;
    public Button SaveButton;
    public InputField NameField;

    public string Name { get; set; }

    private bool inChange = false;
    private string prevName;
    private List<string> propertiesName;
    private List<GameObject> panels;


    private void OnEnable()
    {
        panels = new List<GameObject>();
        propertiesName = DataManager.instance.Properties.Keys.ToList();
        SaveButton.interactable = false;
        NameField.text = "";
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

    public void OpenExistCharacter(string name)
    {
        inChange = true;
        prevName = name;

        var character = DataManager.instance.Characters.First(x => x.Name == name);
        NameField.text = character.Name;
        Name = character.Name;
        if (character.Properties != null)
            foreach (var p in character.Properties)
            {
                OnAddClick();
            }


        for (int i = 0; i < panels.Count; i++)
        {
            var charAdd = panels[i].GetComponent<CharacterAddProperty>();
            var property = character.Properties[i];
            charAdd.SetPropertyName(property.Name);
        }

        for (int i = 0; i < panels.Count; i++)
        {
            var charAdd = panels[i].GetComponent<CharacterAddProperty>();
            var property = character.Properties[i];
            charAdd.SetPropertyValue(property.Type, property.Value);
        }

        SaveButton.interactable = true;
    }

    public void OnChooseName(string name)
    {
        if (name == "")
            SaveButton.interactable = false;
        else if (!DataManager.instance.Characters.Select(x => x.Name).Contains(name))
            SaveButton.interactable = true;
        Name = name;
    }

    public void OnChangePropertyType(string Name)
    {
        foreach (var p in panels.Select(x => x.GetComponent<CharacterAddProperty>()).Where(x => x.Name != Name))
        {
            p.OnChooseNewProperty();
        }
    }

    public List<string> GetMyProperties(string name)
    {
        if (name == "")
            return propertiesName.Except(panels.Select(x => x.GetComponent<CharacterAddProperty>()).Select(x => x.Name)).ToList();
        return propertiesName.Except(panels.Select(x => x.GetComponent<CharacterAddProperty>()).Select(x => x.Name).Where(x => x != name)).ToList();
    }

    public void OnAddClick()
    {
        var panel = Instantiate(Panel, Parent);
        panels.Add(panel);
        if (panels.Count == propertiesName.Count)
            AddPropertyButton.interactable = false;
    }

    public void Delete(string name)
    {
        var panelToDel = panels.Where(x => x.GetComponent<CharacterAddProperty>().Name == name).First();
        Destroy(panelToDel);
        panels.Remove(panelToDel);
        AddPropertyButton.interactable = true;
        OnChangePropertyType("");
    }

    public void Save()
    {
        var propsToAdd = panels.Select(x => x.GetComponent<CharacterAddProperty>())
            .Select(x => { if (x.Value == null || x.Value == "") x.Value = "NotSelected"; return x; })
            .Where(x => (x.Name != "")).ToList();
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


}


