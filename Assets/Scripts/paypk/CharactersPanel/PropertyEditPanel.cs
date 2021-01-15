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

    private int currentIndex;
    private List<GameObject> panels = new List<GameObject>();

    private void OnEnable()
    {
        SaveButton.interactable = true;
    }

    public void AddPanel()
    {
        if (PropertyType != PropertyType.Custom)
            return;
        Debug.Log(currentIndex);
        var panel = Instantiate(Panel, Parent);
        panel.GetComponent<PropertyEditValuePanel>().Index = currentIndex;
        panels.Add(panel);
        currentIndex++;
    }

    public void AddPanel(string value)
    {
        var panel = Instantiate(Panel, Parent);
        panel.GetComponent<PropertyEditValuePanel>().Index = currentIndex;
        panel.GetComponentInChildren<InputField>().text = value;
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
        Debug.Log(currentIndex);
    }

    public void SetName(string name)
    {
        Debug.Log("set name");
        PropertyName = name;
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
        if (PropertyName == "")
        {
            DataManager.instance.Properties.Add(name, new Property());
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
        if (DataManager.instance.Properties.Keys.Contains(name) || name == "")
            SaveButton.interactable = false;
        else
            SaveButton.interactable = true;
    }

    private void ChangeKey(string newName)
    {
        var value = DataManager.instance.Properties[PropertyName];
        DataManager.instance.Properties.Remove(PropertyName);
        DataManager.instance.Properties.Add(newName, value);
    }

    public void ChangeType(int index)
    {
        if (index == 0)
        {
            ClearPanels();
            SaveButton.interactable = false;
            return;
        }
        SaveButton.interactable = true;

        Debug.Log(index);

        PropertyType = (PropertyType)(index - 1);

        Debug.Log(PropertyType);

        if (PropertyType != PropertyType.Custom)
        {
            ClearPanels();
            DataManager.instance.Properties[PropertyName].CustomValues = new List<string>();
        }
            


        DataManager.instance.Properties[PropertyName].Type = PropertyType;
    }


    public void Save()
    {
        
    }

    private void ClearPanels()
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
        if (newValue.Length == 0)
        {
            DataManager.instance.Properties[PropertyName].CustomValues.RemoveAt(index);
            return;
        }

        if (DataManager.instance.Properties[PropertyName].CustomValues == null)
            DataManager.instance.Properties[PropertyName].CustomValues = new List<string>();
        if (index >= DataManager.instance.Properties[PropertyName].CustomValues.Count)
            DataManager.instance.Properties[PropertyName].CustomValues.Add(newValue);
        else
            DataManager.instance.Properties[PropertyName].CustomValues[index] = newValue;
    }
}


