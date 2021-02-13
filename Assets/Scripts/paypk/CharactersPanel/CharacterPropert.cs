using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterPropert : MonoBehaviour
{
    public Text NameText; 
    public string Name { get; set; }

    public void SetName(string name)
    {
        Name = name;
        NameText.text = name;
    }

    public void UpdateCharacterInfoPanel()
    {
        DataManager.instance.CharacterInfoPanel.SetActive(true);
        DataManager.instance.CharacterInfoPanel.GetComponent<CharacterInfoPanel>().UpdatePanel(Name);
    }

    public void Delete()
    {
        DataManager.instance.BaseCharactersPanel.GetComponent<CharactersPanel>().Delete(Name);
    }
}
