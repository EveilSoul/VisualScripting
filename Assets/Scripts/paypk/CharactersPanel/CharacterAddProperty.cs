using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class CharacterAddProperty : MonoBehaviour
{
    public Dropdown Dropdown;

    public Dropdown BoolDropdown;
    public InputField IntField;
    public Dropdown Custom;

    public string Name { get; set; }
    public PropertyType PropertyType { get; set; }
    public string Value { get; set; }
    private Dictionary<string, Property> Properties;
    private List<string> options;
    private bool onSet = false;


    private void OnEnable()
    {
        Properties = DataManager.instance.Properties;
        options = new List<string>();
        Name = "";
        Dropdown.options = GetDropdownList();
    }

    public void SetPropertyName(string name)
    {
        Name = name;
    }

    public void SetPropertyValue(PropertyType propertyType, string value)
    {
        onSet = true;
        PropertyType = propertyType;
        Value = value;
        Dropdown.options = GetDropdownList();
        for (int i = 0; i < Dropdown.options.Count; i++)
        {
            if (Dropdown.options[i].text == Name)
            {
                Dropdown.value = i;
                break;
            }
                
        }

        var properties = DataManager.instance.Properties;

        switch (propertyType)
        {
            case PropertyType.Bool:
                BoolDropdown.gameObject.SetActive(true);
                BoolDropdown.value = (value == "True") ? 1 : 2;
                break;
            case PropertyType.Int:
                IntField.gameObject.SetActive(true);
                IntField.text = value;
                break;
            case PropertyType.Custom:
                Custom.gameObject.SetActive(true);
                var first = new Dropdown.OptionData("NotSelected");
                var options = new List<Dropdown.OptionData>() { first }.Union(properties[Name].CustomValues.Select(x => new Dropdown.OptionData(x))).ToList();
                Custom.options = options;
                Custom.value = GetIndex(options.Select(x => x.text).ToList(), Value);
                break;
        }
    }

    private int GetIndex(List<string> list, string name)
    {
        for (int i=0; i < list.Count; i++)
        {
            if (list[i] == name)
                return i;
        }
        return -1;
    }


    private List<Dropdown.OptionData> GetDropdownList()
    {
        var first = "Select Type";
        options = new List<string>() { first }.Union(DataManager.instance.CharacterAddClass.GetMyProperties(Name)).ToList();
        return options.Select(x => new Dropdown.OptionData(x)).ToList();
    }

    public void OnChooseNewProperty()
    {
        Dropdown.options = GetDropdownList();
        for (int i = 0; i < options.Count; i++)
        {
            if (options[i] == Name)
            {
                onSet = true;
                Dropdown.value = i;
                break;
            }
                
        }
    }

    public void OnTypeChoose(int index)
    {
        if (onSet)
        {
            onSet = false;
            return;
        }
            

        if (index == 0)
        {
            SetAllNoactive();
            return;
        }

        Name = options[index];
        var property = Properties[Name];
        PropertyType = property.Type;
        DataManager.instance.CharacterAddClass.OnChangePropertyType(Name);


        switch (PropertyType)
        {
            case PropertyType.Bool:
                SetAllNoactive();
                BoolDropdown.gameObject.SetActive(true);
                break;
            case PropertyType.Int:
                SetAllNoactive();
                IntField.gameObject.SetActive(true);
                break;
            case PropertyType.Custom:
                SetAllNoactive();
                var first = new Dropdown.OptionData("NotSelected");
                Custom.options = new List<Dropdown.OptionData>() { first }.Union(property.CustomValues.Select(x => new Dropdown.OptionData(x))).ToList();
                Custom.value = 0;
                Custom.gameObject.SetActive(true);
                break;
        }
    }

    public void OnCustomValueChoose(int index)
    {
        Value = Custom.options[index].text;
    }

    public void OnIntValueChoose(string value)
    {
        Value = value;
    }

    public void OnBoolValueChoose(int index)
    {
        Value = (index == 1) ? "True" : (index == 2) ? "False" : "";
    }

    private void SetAllNoactive()
    {
        BoolDropdown.gameObject.SetActive(false);
        IntField.gameObject.SetActive(false);
        Custom.gameObject.SetActive(false);
    }

    public void Delete()
    {
        DataManager.instance.CharacterAddClass.Delete(Name);
    }
}


