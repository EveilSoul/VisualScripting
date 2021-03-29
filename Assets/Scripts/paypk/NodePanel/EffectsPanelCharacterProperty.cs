using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EffectsPanelCharacterProperty : MonoBehaviour
{
    public int ID = -1;

    public Dropdown Dropdown;

    public Dropdown Operation;
    public InputField IntField;
    public Dropdown BoolDropdown;
    public Dropdown CustomDropdown;

    public string ParentName { get; set; }
    public string Name { get; set; }
    public PropertyType PropertyType { get; set; }
    public string OperationValue { get; set; }
    public string Value { get; set; }
    private Dictionary<string, Property> Properties;
    private List<string> options;
    private bool onSet = false;
    private List<string> operations = new List<string>() { "=", "+", "-" };


    public void Initial(string name)
    {
        Properties = DataManager.instance.Properties;
        options = new List<string>();
        ParentName = name;
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
        if (propertyType == PropertyType.Int)
        {
            OperationValue = value[0].ToString();
            Value = value.Substring(1);
            Operation.gameObject.SetActive(true);
            Operation.options = operations.Select(x => new Dropdown.OptionData(x)).ToList();
            for (int i = 0; i < operations.Count; i++)
            {
                if (operations[i] == OperationValue)
                {
                    Operation.value = i;
                    break;
                }

            }
        }
        else
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
                CustomDropdown.gameObject.SetActive(true);
                var options = properties[Name].CustomValues.Select(x => new Dropdown.OptionData(x)).ToList();
                CustomDropdown.options = options;
                CustomDropdown.value = GetIndex(options.Select(x => x.text).ToList(), Value);
                break;
        }
    }

    private int GetIndex(List<string> list, string name)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] == name)
                return i;
        }
        return -1;
    }


    private List<Dropdown.OptionData> GetDropdownList()
    {
        var first = DataManager.SelectTypeValue;
        options = new List<string>() { first }.Union(NodeData.instance.GetMyNodeData(ref ID, transform).EffectsPanel.GetComponent<EffectsPanel>().GetMyProperties(ParentName, Name)).ToList();
        return options.Select(x => new Dropdown.OptionData(x)).ToList();
    }

    public void OnChooseNewProperty()
    {
        Dropdown.options = GetDropdownList();
        for (int i = 0; i < options.Count; i++)
        {
            if (options[i] == Name)
            {
                Dropdown.value = i;
                break;
            }

        }
    }

    public void OnTypeChoose(int index)
    {
        if (onSet)
            return;

        if (index == 0)
        {
            SetAllNoactive();
            return;
        }

        Name = options[index];
        var property = Properties[Name];
        PropertyType = property.Type;
        NodeData.instance.GetMyNodeData(ref ID, transform).EffectsPanel.GetComponent<EffectsPanel>().OnChangePropertyType(ParentName, Name);


        switch (PropertyType)
        {
            case PropertyType.Bool:
                SetAllNoactive();
                BoolDropdown.gameObject.SetActive(true);
                break;
            case PropertyType.Int:
                SetAllNoactive();
                Operation.gameObject.SetActive(true);
                IntField.gameObject.SetActive(true);
                break;
            case PropertyType.Custom:
                SetAllNoactive();
                var first = new Dropdown.OptionData("Select Value");
                CustomDropdown.options = new List<Dropdown.OptionData>() { first }.Union(property.CustomValues.Select(x => new Dropdown.OptionData(x))).ToList();
                CustomDropdown.value = 0;
                CustomDropdown.gameObject.SetActive(true);
                break;
        }
    }

    public void OnCustomValueChoose(int index)
    {
        Value = CustomDropdown.options[index].text;
        if (index == 0)
            Value = "";
    }

    public void OnOperationValueChoose(int index)
    {
        switch (index)
        {
            case 0:
                OperationValue = "=";
                break;
            case 1:
                OperationValue = "+";
                break;
            case 2:
                OperationValue = "-";
                break;
        }
        
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
        CustomDropdown.gameObject.SetActive(false);
        Operation.gameObject.SetActive(false);
    }

    public void Delete()
    {
        NodeData.instance.GetMyNodeData(ref ID, transform).EffectsPanel.GetComponent<EffectsPanel>().DeleteProperty(ParentName, Name);
    }
}
