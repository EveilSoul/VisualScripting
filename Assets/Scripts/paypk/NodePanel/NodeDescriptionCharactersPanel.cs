using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class NodeDescriptionCharactersPanel : MonoBehaviour
{
    public int ID = -1;

    public GameObject Character;
    public Transform Parent;
    public GameObject AddButton;
    public InputField TextField;

    private string text;

    private List<GameObject> panels = new List<GameObject>();
    public List<string> characterNames = new List<string>();

    public List<string> GetNotSelectedCharacters() => DataManager.instance.Characters.Select(x => x.Name).Except(characterNames).ToList();
    public List<string> GetSelectedCharactersToCondition() => characterNames.Except(NodeData.instance.GetMyNodeData(ref ID, transform).ConditionPanel.GetComponent<ConditionPanel>().Names).ToList();
    public List<string> GetSelectedCharactersToEffects() => characterNames.Except(NodeData.instance.GetMyNodeData(ref ID, transform).EffectsPanel.GetComponent<EffectsPanel>().Names).ToList(); 

    public void AddCharacter(string name)
    {
        var panel = Instantiate(Character, Parent);
        panel.transform.SetSiblingIndex(Parent.childCount - 2);
        panel.GetComponent<NodeDescriptionCharacter>().SetName(name);
        panels.Add(panel);
        characterNames.Add(name);
        if (DataManager.instance.Characters.Count == characterNames.Count)
            AddButton.SetActive(false);
    }

    public void Delete(string name)
    {
        var panel = panels.First(x => x.GetComponent<NodeDescriptionCharacter>().Name == name);
        panels.Remove(panel);
        Destroy(panel);
        characterNames.Remove(name);
        AddButton.SetActive(true);
    }

    public void OnTextChange(string text)
    {
        this.text = text;
        TextField.text = text;
    }

    public void Save(int id)
    {
        if (DataManager.instance.Nodes.Select(x => x.Id).Contains(id))
            DataManager.instance.Nodes.Remove(DataManager.instance.Nodes.First(x => x.Id == id));

        var node = new Saving_Node();
        var condition = new Condition();
        condition.NodeCharacters = new List<NodeCharacter>();
        var effect = new Effect();
        effect.NodeCharacters = new List<NodeCharacter>();
        var quest = new Quest();

        node.Id = id;
        node.CharacterNames = characterNames;
        node.Text = text;

        foreach (var p in NodeData.instance.GetMyNodeData(ref ID, transform).ConditionPanel.GetComponent<ConditionPanel>().panels)
        {
            var nodeCharacter = new NodeCharacter();
            var charPanel = p.GetComponent<ConditionCharacterPanel>();
            nodeCharacter.Name = charPanel.Name;
            nodeCharacter.PropertyValues = new List<PropertyValue>();
            foreach (var pp in charPanel.panels)
            {
                var propValue = new PropertyValue();
                var charProp = pp.GetComponent<ConditionPanelCharacterProperty>();
                propValue.Name = charProp.Name;
                propValue.PropertyType = charProp.PropertyType;
                propValue.Value = (charProp.PropertyType == PropertyType.Int && charProp.OperationValue == null)? "=" + charProp.Value: charProp.OperationValue + charProp.Value;
                nodeCharacter.PropertyValues.Add(propValue);
            }
            condition.NodeCharacters.Add(nodeCharacter);
        }

        foreach (var p in NodeData.instance.GetMyNodeData(ref ID, transform).EffectsPanel.GetComponent<EffectsPanel>().panels)
        {
            var nodeCharacter = new NodeCharacter();
            var charPanel = p.GetComponent<EffectsCharacterPanel>();
            nodeCharacter.Name = charPanel.Name;
            nodeCharacter.PropertyValues = new List<PropertyValue>();
            foreach (var pp in charPanel.panels)
            {
                var propValue = new PropertyValue();
                var charProp = pp.GetComponent<EffectsPanelCharacterProperty>();
                propValue.Name = charProp.Name;
                propValue.PropertyType = charProp.PropertyType;
                propValue.Value = (charProp.PropertyType == PropertyType.Int && charProp.OperationValue == null) ? "=" + charProp.Value : charProp.OperationValue + charProp.Value;
                nodeCharacter.PropertyValues.Add(propValue);
            }
            effect.NodeCharacters.Add(nodeCharacter);
        }

        var chooseQuestPanel = NodeData.instance.GetMyNodeData(ref ID, transform).ChooseQuestPanel;
        quest = new Quest()
        {
            Name = chooseQuestPanel.QuestNameDropdown.options[chooseQuestPanel.QuestNameDropdown.value].text,
            Type = (QuestType)(chooseQuestPanel.QuestTypeDropdown.value)
        };

        node.Condition = condition;
        node.Effect = effect;
        node.Quest = quest;

        DataManager.instance.Nodes.Add(node);
        node.Initial();

        ClearAll();
    }

    public void ClearAll()
    {
        NodeData.instance.GetMyNodeData(ref ID, transform).ConditionPanel.GetComponent<ConditionPanel>().ClearAll();
        NodeData.instance.GetMyNodeData(ref ID, transform).EffectsPanel.GetComponent<EffectsPanel>().ClearAll();
        //TextField.text = "";
        foreach (var p in panels)
        {
            Destroy(p);
        }
        panels = new List<GameObject>();
        characterNames = new List<string>();
    }

    public void OpenExistNode(int id)
    {
        if (!DataManager.instance.Nodes.Select(x => x.Id).Contains(id))
            return;

        if (DataManager.instance.Characters.Count > characterNames.Count)
            AddButton.SetActive(true);

        var node = DataManager.instance.Nodes.First(x => x.Id == id);
        foreach (var c in node.CharacterNames)
        {
            AddCharacter(c);
        }

        text = node.Text;
        TextField.text = node.Text;

        foreach (var p in node.Condition.NodeCharacters)
        {
            NodeData.instance.GetMyNodeData(ref ID, transform).ConditionPanel.GetComponent<ConditionPanel>().OpenExist(p.Name, p.PropertyValues);
        }

        foreach (var p in node.Effect.NodeCharacters)
        {
            NodeData.instance.GetMyNodeData(ref ID, transform).EffectsPanel.GetComponent<EffectsPanel>().OpenExist(p.Name, p.PropertyValues);
        }
    }
}
