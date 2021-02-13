using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PropertyValue
{
    public string Name;
    public PropertyType PropertyType;
    public string Value;
}

public struct NodeCharacter
{
    public string Name;
    public List<PropertyValue> PropertyValues;
}

public struct Condition
{
    public List<NodeCharacter> NodeCharacters;
}

public struct Effect
{
    public List<NodeCharacter> NodeCharacters;
}

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
        foreach(var e in Condition.NodeCharacters)
        {
            for (var i=0;i< e.PropertyValues.Count;i++)
            {
                if (e.PropertyValues[i].Name == lastName)
                {
                    var newProp = e.PropertyValues[i];
                    newProp.Name = newName;
                    e.PropertyValues[i] = newProp;
                }
            }
        }
        foreach (var e in Effect.NodeCharacters)
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
        foreach (var e in Condition.NodeCharacters)
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
        foreach (var e in Effect.NodeCharacters)
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
        foreach (var e in Condition.NodeCharacters)
        {
            for (var i = 0; i < e.PropertyValues.Count; i++)
            {
                if (e.PropertyValues[i].Name == propName)
                {
                    var newProp = e.PropertyValues[i];
                    newProp.Value = "NotSelected";
                    e.PropertyValues[i] = newProp;
                }
            }
        }
        foreach (var e in Effect.NodeCharacters)
        {
            for (var i = 0; i < e.PropertyValues.Count; i++)
            {
                if (e.PropertyValues[i].Name == propName)
                {
                    var newProp = e.PropertyValues[i];
                    newProp.Value = "NotSelected";
                    e.PropertyValues[i] = newProp;
                }
            }
        }
    }
}


