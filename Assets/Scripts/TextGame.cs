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

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPlayButtonClick()
    {
        var node = ConnectionManager.GetStartNode();
        DisplayAction(node);
    }

    public void OnAchionSelected(GameObject choice)
    {
        var id = int.Parse(choice.GetComponentsInChildren<Text>(true).First(x => x.name == "ID").text);
        DisplayAction(GraphController.Nodes[id]);
    }

    public static void DisplayAction(Node node)
    {
        instance.gameObject.SetActive(true);

        var text = node.Panel.GetComponentsInChildren<InputField>(true).First(x => x.name == "ActionDescription").text;
        var childs = ConnectionManager.GetNodeChilds(node.GetComponent<Connection>()).ToList();
        var variants = childs.Select(x => x.Node.Panel.GetComponentInChildren<BranchDescription>(true).Text.text).ToList();

        instance.MainText.text = text;

        if (variants.Where(x => x != "").Count() == 0)
            variants = new List<string>() { "Далее" };

        var transformChildCount = instance.ButtonsPanel.childCount;
        Enumerable.Range(0, transformChildCount).ToList().ForEach(i => Destroy(instance.ButtonsPanel.GetChild(i).gameObject));

        for(int i = 0; i < childs.Count; i++)
        {
            var button = Instantiate(instance.ChoicePrefab, instance.ButtonsPanel);
            var allText = button.GetComponentsInChildren<Text>(true);
            allText.First(x => x.name == "Text").text = variants[i];
            allText.First(x => x.name == "ID").text = childs[i].Node.Id.ToString();
        }
    }
}
