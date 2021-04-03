using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TextGame : MonoBehaviour
{
    private static TextGame instance;

    public Text MainText;
    public Button ChoicePrefab;
    public Transform ButtonsPanel;

    private Node displayedNode;

    public GameObject CharacterPrefab;
    public Transform CharacterPanel;
    public Text Properties;
    public Color SelectedCharacterColor;
    private Color NormalCharaterColor;

    private Button selectedCharacterButton;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        NormalCharaterColor = CharacterPrefab.GetComponent<Button>().colors.normalColor;

        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPlayButtonClick()
    {
        var node = ConnectionManager.GetStartNode();
        node.WorldState = DataManager.instance.Characters.Clone();
        node.ApplyEffects();
        instance.displayedNode = node;
        DisplayAction();
    }

    public void OnAchionSelected(GameObject choice)
    {
        var id = int.Parse(choice.GetComponentsInChildren<Text>(true).First(x => x.name == "ID").text);
        instance.displayedNode = GraphController.Nodes[id];
        instance.displayedNode.ApplyEffects();
        DisplayAction();
    }

    public void DisplayWorldState()
    {
        var count = CharacterPanel.childCount;
        GameObject first = null;
        Enumerable.Range(0, count).ToList().ForEach(i => Destroy(CharacterPanel.GetChild(i).gameObject));

        foreach(var character in displayedNode.WorldState)
        {
            var obj = Instantiate(CharacterPrefab, CharacterPanel);
            obj.GetComponentInChildren<Text>().text = character.Name;

            if (first == null)
                first = obj;
        }

        if (displayedNode.WorldState.Count > 0)
        {
            OnCharacterSelected(first.GetComponentInChildren<Text>());
        }
    }

    public void OnCharacterSelected(Text charName)
    {
        instance.DisplayCharacterProperties(charName.text);
        Button button = charName.GetComponentInParent<Button>();
        var colors = button.colors;
        colors.normalColor = colors.selectedColor = instance.SelectedCharacterColor;
        button.colors = colors;

        if (instance.selectedCharacterButton != null)
        {
            colors = instance.selectedCharacterButton.colors;
            colors.normalColor = colors.selectedColor = instance.NormalCharaterColor;
            instance.selectedCharacterButton.colors = colors;
        }
        instance.selectedCharacterButton = button;
    }

    public void DisplayCharacterProperties(string name)
    {
        Character character = displayedNode.WorldState.FirstOrDefault(x => x.Name == name);
        Properties.text = character.Name;
        foreach (var p in character.Properties)
            Properties.text += $"\n{p.Name}:{p.Value}"; 
    }

    public static void DisplayAction()
    {
        instance.gameObject.SetActive(true);

        var text = instance.displayedNode.Panel.GetComponentsInChildren<InputField>(true).First(x => x.name == "ActionDescription").text;
        var childs = ConnectionManager.GetNodeChilds(instance.displayedNode.GetComponent<Connection>())
            .Where(x => { x.Node.WorldState = instance.displayedNode.WorldState.Clone(); return x.Node.CheckCondition(); })
            .ToList();
        var variants = childs.Select(x => x.Node.Panel.GetComponentInChildren<BranchDescription>(true).Text.text).ToList();

        instance.MainText.text = text;

        if (variants.Where(x => x != "").Count() == 0)
            variants = new List<string>() { "Далее" };

        var transformChildCount = instance.ButtonsPanel.childCount;
        Enumerable.Range(0, transformChildCount).ToList().ForEach(i => Destroy(instance.ButtonsPanel.GetChild(i).gameObject));

        for (int i = 0; i < childs.Count; i++)
        {
            var button = Instantiate(instance.ChoicePrefab, instance.ButtonsPanel);
            var allText = button.GetComponentsInChildren<Text>(true);
            allText.First(x => x.name == "Text").text = variants[i];
            allText.First(x => x.name == "ID").text = childs[i].Node.Id.ToString();
        }

        instance.DisplayWorldState();
    }
}
