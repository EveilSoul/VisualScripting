using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class CharactersEvents
{
    public static void Initial()
    {
        DataManager.instance.OnDeleteProperty += DeleteProperty;
        DataManager.instance.OnChangePropertyName += ChangePropertyName;
        DataManager.instance.OnChangePropertyValue += ChangePropertyValueName;
        DataManager.instance.OnDeletePropertyValue += DeletePropertyValue;
    }

    public static void DeleteProperty(string name)
    {
        DataManager.instance.Characters.ForEach(x => x.Properties.RemoveAll(y => y.Name == name));
    }

    public static void ChangePropertyName(string lastName, string newName)
    {
        foreach(var c in DataManager.instance.Characters)
        {
           for(int i = 0; i<c.Properties.Count;i++)
            {
                if(c.Properties[i].Name == lastName)
                {
                    var p = c.Properties[i];
                    p.Name = newName;
                    c.Properties[i] = p;
                }
            }
        }
    }

    public static void ChangePropertyValueName(string propName, string lastName, string newName)
    {
        foreach (var c in DataManager.instance.Characters)
        {
            for (int i = 0; i < c.Properties.Count; i++)
            {
                if (c.Properties[i].Name == propName)
                {
                    if (c.Properties[i].Value == lastName)
                        c.Properties[i].Value = newName;
                }
            }
        }
    }

    public static void DeletePropertyValue(string propName)
    {
        foreach (var e in DataManager.instance.Characters)
        {
            for (int i = 0; i < e.Properties.Count; i++)
            {
                if (e.Properties[i].Name == propName)
                    e.Properties[i].Value = "NotSelected";
            }
        }
    }

    
}
