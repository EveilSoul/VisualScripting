using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class DataManager : MonoBehaviour
{
    public static DataManager instance;

    public GameObject BaseCharactersPanel;
    public GameObject PropertyEditPanel;
    public GameObject PropertiesListPanel;
    public CharacterAddPanel CharacterAddClass;
    public GameObject CharacterInfoPanel;

    #region events
    public event Action<string> OnDeleteProperty;
    public void OnDeletePropertyInvoke(string name)
    {
        OnDeleteProperty.Invoke(name);
    }

    public event Action<string, string> OnChangePropertyName;
    public void OnChangePropertyNameInvoke(string lastName, string newName)
    {
        OnChangePropertyName.Invoke(lastName, newName);
    }

    public event Action<string, string, string> OnChangePropertyValue;
    public void OnChangePropertyValueInvoke(string propName, string lastName, string newName)
    {
        OnChangePropertyValue.Invoke(propName, lastName, newName);
    }

    public event Action<string> OnDeletePropertyValue;
    public void OnDeletePropertyValueInvoke(string name)
    {
        OnDeletePropertyValue.Invoke(name);
    }
    #endregion


    private void Awake()
    {
        instance = this;
        CharactersEvents.Initial();
    }


    public List<Saving_Node> Nodes = new List<Saving_Node>();

    public List<Character> Characters = new List<Character>();
    //{
    //    new Character(){ Name = "first"} ,
    //    new Character(){ Name = "Sec"}
    //};
    public Dictionary<string, Property> Properties = new Dictionary<string, Property>();
    //{
    //    { "one", new Property { Type = PropertyType.Custom, CustomValues = new List<string>() {"rr", "rrr" } } },
    //    { "two", new Property { Type = PropertyType.Custom, CustomValues = new List<string>() {"tt", "rtttrr" } } },
    //    { "rrrtttt", new Property { Type = PropertyType.Int } },
    //    { "rtvvv", new Property { Type = PropertyType.Bool } }
    //};
}

