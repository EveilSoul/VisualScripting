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

    #region Panels
    public GameObject BaseCharactersPanel;
    public GameObject PropertyEditPanel;
    public GameObject PropertiesListPanel;
    public CharacterAddPanel CharacterAddClass;
    public GameObject CharacterInfoPanel;
    #endregion

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

    public static string NodePanelTag = "Node-Panel";
    public static string NotSelectedValue = "Not Selected";
    public static string SelectTypeValue = "Select Type";


    private void Awake()
    {
        instance = this;
        CharactersEvents.Initial();
    }


    public List<Saving_Node> Nodes = new List<Saving_Node>();

    public List<Character> Characters = new List<Character>();
    public Dictionary<string, Property> Properties = new Dictionary<string, Property>();
}

