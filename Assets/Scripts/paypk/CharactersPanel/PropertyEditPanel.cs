using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class PropertyEditPanel : MonoBehaviour
{
    public InputField Name;
    public Dropdown Type;
    public GameObject Panel;
    public Transform Parent;
    public Button SaveButton;

    public string PropertyName { get; set; }
    public PropertyType PropertyType { get; set; }
    public List<string> PropertyValues { get; set; }

    private int currentIndex;
    private string prevName;
    private bool isExist = false;
    private List<GameObject> panels = new List<GameObject>();

    private void OnEnable()
    {
        SaveButton.interactable = true;
    }

    public void AddPanel()
    {
        if (PropertyType != PropertyType.Custom)
            return;
        var panel = Instantiate(Panel, Parent);
        panel.GetComponent<PropertyEditValuePanel>().Index = currentIndex;
        panels.Add(panel);
        currentIndex++;
    }

    public void AddPanel(string name)
    {
        var panel = Instantiate(Panel, Parent);
        panel.GetComponent<PropertyEditValuePanel>().Index = currentIndex;
        panel.GetComponent<PropertyEditValuePanel>().Name = name;
        panel.GetComponentInChildren<InputField>().text = name;
        panels.Add(panel);
        currentIndex++;
    }

    public void AddProperty()
    {
        ClearPanels();
        Name.text = "";
        PropertyName = "";
        Type.value = 0;
        currentIndex = DataManager.instance.Properties.Count;
    }

    public void SetName(string name)
    {
        isExist = true;
        PropertyName = name;
        prevName = name;
        Name.text = name;
        var propType = DataManager.instance.Properties[name].Type;
        Type.value = (int)(propType + 1);
        ClearPanels();
        if (propType == PropertyType.Custom)
            AddPanels(name);
        SaveButton.interactable = true;
    }

    private void AddPanels(string name)
    {
        foreach (var p in DataManager.instance.Properties[name].CustomValues)
        {
            AddPanel(p);
        }
    }

    public void ChangeName(string name)
    {
        if (name != prevName && DataManager.instance.Properties.Keys.Contains(name))
        {
            Type.interactable = false;
            SaveButton.interactable = false;
            return;
        }

        Type.interactable = true;

        if (PropertyName == "")
        {
            //DataManager.instance.Properties.Add(name, new Property());
            PropertyValues = new List<string>();
            PropertyName = name;
        }
        else
        {
            ChangeKey(name);
            PropertyName = name;
        }
    }

    public void OnNameChanged(string name)
    {
        if (name != prevName && DataManager.instance.Properties.Keys.Contains(name) || name == "")
            SaveButton.interactable = false;
        else
            SaveButton.interactable = true;
    }

    private void ChangeKey(string newName)
    {
        var value = DataManager.instance.Properties[PropertyName];
        DataManager.instance.Properties.Remove(PropertyName);
        DataManager.instance.Properties.Add(newName, value);
        DataManager.instance.OnChangePropertyNameInvoke(PropertyName, newName);
    }

    public void ChangeType(int index)
    {
        if (index == 0)
        {
            ClearPanels();
            SaveButton.interactable = false;
            return;
        }
        if (!DataManager.instance.Properties.Keys.Contains(PropertyName))
            SaveButton.interactable = true;


        PropertyType = (PropertyType)(index - 1);

        if (PropertyType != PropertyType.Custom && isExist)
        {
            ClearPanels();
            if (DataManager.instance.Properties.Keys.Contains(PropertyName))
                DataManager.instance.Properties[PropertyName].CustomValues = new List<string>();
        }
           
    }

    public void ClearPanels()
    {
        foreach (var p in panels)
        {
            Destroy(p);
        }
        panels = new List<GameObject>();
        currentIndex = 0;
    }

    public void OnPropertyEdit(int index, string newValue)
    {
        if (!isExist)
        {
            if (index >= PropertyValues.Count)
                PropertyValues.Add(newValue);
            else
                PropertyValues[index] = newValue;

            return;
        }

        if (newValue.Length == 0 && DataManager.instance.Properties[PropertyName].CustomValues.Count > index)
        {
            DataManager.instance.Properties[PropertyName].CustomValues.RemoveAt(index);
            DataManager.instance.OnDeletePropertyValueInvoke(PropertyName);
            return;
        }

        if (DataManager.instance.Properties[PropertyName].CustomValues == null)
            DataManager.instance.Properties[PropertyName].CustomValues = new List<string>();

        if (index >= DataManager.instance.Properties[PropertyName].CustomValues.Count)
            DataManager.instance.Properties[PropertyName].CustomValues.Add(newValue);
        else
        {
            // Update data
            DataManager.instance.OnChangePropertyValueInvoke(PropertyName, DataManager.instance.Properties[PropertyName].CustomValues[index], newValue);
            DataManager.instance.Properties[PropertyName].CustomValues[index] = newValue;
        }
    }

    public void Save()
    {
        if (DataManager.instance.Properties.Keys.Contains(PropertyName))
        {
            DataManager.instance.Properties[PropertyName].Type = PropertyType;
            DataManager.instance.Properties[PropertyName].CustomValues = PropertyValues;
        }
        else
        {
            DataManager.instance.Properties.Add(PropertyName, new Property()
            {
                Type = PropertyType,
                CustomValues = PropertyValues
            });
        }
        isExist = false;
    }
}


