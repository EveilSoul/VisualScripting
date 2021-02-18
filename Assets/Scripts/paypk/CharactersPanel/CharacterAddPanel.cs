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

    private bool isOpenedExist = false;
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
        isOpenedExist = false;
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
        isOpenedExist = true;
        prevName = name;

        var character = DataManager.instance.Characters.First(x => x.Name == name);
        NameField.text = character.Name;
        Name = character.Name;
        if (character.Properties != null)
        {
            foreach (var p in character.Properties)
            {
                CreateNewPanel();
            }
        }

        // Для того, чтобы корректно отображались возможные типы свойсв необходимо сначала проинициализировать имена 
        // т.к. список доступных свойств рассчитывается из списка всех имен и текущего
        for (int i = 0; i < panels.Count; i++)
        {
            var charAdd = panels[i].GetComponent<CharacterAddProperty>();
            var property = character.Properties[i];
            charAdd.SetName(property.Name);
        }

        for (int i = 0; i < panels.Count; i++)
        {
            var charAdd = panels[i].GetComponent<CharacterAddProperty>();
            var property = character.Properties[i];
            charAdd.SetPropertyValues(property.Type, property.Value);
        }

        SaveButton.interactable = true;
    }

    public void OnChangeName(string name)
    {
        if (name == "" || DataManager.instance.Characters.Select(x => x.Name).Contains(name))
            SaveButton.interactable = false;
        else
        {
            SaveButton.interactable = true;
            Name = name;
        } 
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

    public void CreateNewPanel()
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
            // Заменяем все неинициализированные значения на "Не выбрано"
            .Select(x => { if (x.Value == null || x.Value == "") x.Value = DataManager.NotSelectedValue; return x; })
            .Where(x => (x.Name != "")).ToList();
        if (Name != "")
        {
            // Если открыт уже существующий, меняем его значения. Иначе создаем нового.
            if (isOpenedExist)
            {
                var character = DataManager.instance.Characters.Where(x => x.Name == prevName).First();
                character.Name = Name;
                character.Properties = propsToAdd.Select(x => new CharacterProperty() { Name = x.Name, Type = x.PropertyType, Value = x.Value }).ToList();
                isOpenedExist = false;
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


