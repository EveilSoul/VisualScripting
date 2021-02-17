using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PropertyValue
{
    public string Name;
    public PropertyType PropertyType;
    public string Value;
}

[System.Serializable]
public struct NodeCharacter
{
    public string Name;
    public List<PropertyValue> PropertyValues;
}

[System.Serializable]
public struct Condition
{
    public List<NodeCharacter> NodeCharacters;
}

[System.Serializable]
public struct Effect
{
    public List<NodeCharacter> NodeCharacters;
}

[System.Serializable]
public struct Saving_Node
{
    public int Id;
    public List<string> CharacterNames;
    public Condition Condition;
    public string Text;
    public Effect Effect;

    public void Initial()
    {
        DataManager.instance.OnDeleteProperty += DeleteProperty;
        DataManager.instance.OnChangePropertyName += ChangePropertyName;
        DataManager.instance.OnChangePropertyValue += ChangePropertyValueName;
        DataManager.instance.OnDeletePropertyValue += DeletePropertyValue;
    }

    public void DeleteProperty(string name)
    {
        Condition.NodeCharacters.ForEach(x => x.PropertyValues.RemoveAll(y => y.Name == name));
        Effect.NodeCharacters.ForEach(x => x.PropertyValues.RemoveAll(y => y.Name == name));
    }

    public void ChangePropertyName(string lastName, string newName)
    {
        ChangePropertyName(lastName, newName, Condition.NodeCharacters);
        ChangePropertyName(lastName, newName, Effect.NodeCharacters);
    }

    private void ChangePropertyName(string lastName, string newName, List<NodeCharacter> nodeCharacters)
    {
        foreach (var e in nodeCharacters)
        {
            for (var i = 0; i < e.PropertyValues.Count; i++)
            {
                if (e.PropertyValues[i].Name == lastName)
                {
                    var newProp = e.PropertyValues[i];
                    newProp.Name = newName;
                    e.PropertyValues[i] = newProp;
                }
            }
        }
    }

    public void ChangePropertyValueName(string propName, string lastName, string newName)
    {
        ChangePropertyValueName(propName, lastName, newName, Condition.NodeCharacters);
        ChangePropertyValueName(propName, lastName, newName, Effect.NodeCharacters);
    }

    private void ChangePropertyValueName(string propName, string lastName, string newName, List<NodeCharacter> nodeCharacters)
    {
        foreach (var e in nodeCharacters)
        {
            for (var i = 0; i < e.PropertyValues.Count; i++)
            {
                if (e.PropertyValues[i].Name == propName)
                {
                    if (e.PropertyValues[i].Value == lastName)
                    {
                        var newProp = e.PropertyValues[i];
                        newProp.Value = newName;
                        e.PropertyValues[i] = newProp;
                    }
                }
            }
        }
    }

    public void DeletePropertyValue(string propName)
    {
        DeletePropertyValue(propName, Condition.NodeCharacters);
        DeletePropertyValue(propName, Effect.NodeCharacters);
    }

    private void DeletePropertyValue(string propName, List<NodeCharacter> nodeCharacters)
    {
        foreach (var e in nodeCharacters)
        {
            for (var i = 0; i < e.PropertyValues.Count; i++)
            {
                if (e.PropertyValues[i].Name == propName)
                {
                    var newProp = e.PropertyValues[i];
                    newProp.Value = DataManager.NotSelectedValue;
                    e.PropertyValues[i] = newProp;
                }
            }
        }
    }
}


