using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInfoPanel : MonoBehaviour
{
    public Text Name;
    public Text Properties;

    public void UpdatePanel(string name)
    {
        var character = DataManager.instance.Characters.Where(x => x.Name == name).First();
        SetName(character.Name);
        SetPropertiesText(character.Properties);
    }

    public void SetName(string name)
    {
        Name.text = name;
    }

    public void SetPropertiesText(List<CharacterProperty> properties)
    {
        if (properties == null)
        {
            Properties.text = "";
            return;
        }
        var result = new StringBuilder();

        foreach (var p in properties)
        {
            result.Append(p.Name);
            result.Append(": ");
            result.Append(p.Value);
            result.Append("\n");
        }

        Properties.text = result.ToString();
    }

    public void OnEditClick()
    {
        DataManager.instance.CharacterAddClass.OpenExistCharacter(Name.text);
    }
}
