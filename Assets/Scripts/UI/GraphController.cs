using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GraphController : MonoBehaviour
{
    public float MinScroll;
    public float MaxScroll;
    public float Sencivity;

    public Color NormalColor;
    public static Color NormalOutlineColor => instance.NormalColor;
    public Color ErrorColor;
    public static Color ErrorOutlineColor => instance.ErrorColor;

    private static GraphController instance;

    public static Vector3 MousePosition => instance.GetMP();

    public Vector3 GetMP()
    {
        ScrollRect scrollRect = Background.GetComponentInParent<ScrollRect>();
        Vector2 sizeDelta = instance.Background.GetComponent<RectTransform>().sizeDelta;
        float scroll = currentScroll;
        var offset = new Vector3((1 - scrollRect.horizontalScrollbar.value) * sizeDelta.x - scroll, (1 - scrollRect.verticalScrollbar.value) * sizeDelta.y - scroll / 2) * PanelScroll * 0.85f;
        Debug.Log(offset);
        return Input.mousePosition - new Vector3(Screen.width, Screen.height) / 2 - offset / 2;
    }

    public GameObject ActionNodePrefab;
    public GameObject RootNodePrefab;
    public GameObject Background;
    public GameObject PanelBackground;
    public GameObject NodePanelPrefab;
    public GameObject RootPanel;

    public GameObject CreationMenu;
    public GameObject NodeMenu;

    public Vector2 DuplicateOffset;

    public Button PlayButton;

    public static bool IsPointerOutMenu;
    public static bool IsPointerOutNode = true;

    public static Dictionary<int, Node> Nodes;
    public static Dictionary<int, NodeData> NodeData;

    private Outline selectedNode;

    public float currentScroll;

    public static float PanelScroll => instance.MinScroll / instance.currentScroll;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        CreationMenu.SetActive(false);
        NodeMenu.SetActive(false);
        DataStorage.OnSceneClearing += OnSceneClearing;
        Nodes = new Dictionary<int, Node>();
        NodeData = new Dictionary<int, NodeData>();
    }

    [ContextMenu("Reset Nodes ID")]
    public void ResetNodesId()
    {
        PlayerPrefs.SetInt("NodeId", 1);
    }

    public static string GetAllNodeTextById(int id)
    {
        var result = new List<string>();
        var curent = Nodes[id].GetComponent<Connection>();

        do
        {
            if (curent.GetComponent<RootNode>() == null)
                result.Add(curent.Node.Panel.GetComponentsInChildren<InputField>(true).First(x => x.name == "ActionDescription").text + '\n');

            var parents = ConnectionManager.GetNodeParents(curent);
            if (parents.Count > 0)
                curent = parents[0];
            else curent = null;
        } while (curent != null);
        result.Reverse();
        return result.Aggregate((x, y) => x + y);
    }

    public static void OnNodePointerClick(Node node)
    {
        if (instance.selectedNode == null || node.gameObject != instance.selectedNode.gameObject)
        {
            if (instance.selectedNode != null)
            {
                instance.selectedNode.enabled = false;
            }
            instance.selectedNode = node.GetComponent<Outline>();
        }
        instance.selectedNode.enabled = true;
        instance.selectedNode.effectColor = instance.NormalColor;
    }

    internal static void Reset()
    {
        Nodes = new Dictionary<int, Node>();
        InstantiateRootNode();
    }

    public static void AddNodes(NodesData data)
    {
        //foreach (var node in FindObjectsOfType<Node>())
        foreach (var node in Nodes.Values)
        {
            Storage_NodeInfo info = new Storage_NodeInfo() { Id = node.Id, Position = node.transform.position, IsRootNode = node.GetComponent<RootNode>() != null, Name = node.Name };
            data.Nodes.Add(info);
        }
    }

    public void OnSceneClearing()
    {
        selectedNode = null;
        IsPointerOutNode = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (IsPointerOutNode)
                EnableCreationMenu();
            else EnableNodeMenu();
        }

        if (CreationMenu.activeInHierarchy && (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(0) && IsPointerOutMenu))
        {
            CreationMenu.SetActive(false);
        }

        if (NodeMenu.activeInHierarchy && (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(0) && IsPointerOutMenu))
        {
            HideNodeMenu();
        }

        if (Input.GetMouseButtonDown(0) && IsPointerOutNode)
        {
            if (selectedNode != null)
            {
                selectedNode.enabled = false;
            }
        }

        var scroll = Input.GetAxis("Mouse ScrollWheel");
        currentScroll += scroll * Sencivity;

        currentScroll = Mathf.Clamp(currentScroll, MinScroll, MaxScroll);
        Background.GetComponent<RectTransform>().sizeDelta = new Vector2(currentScroll * 2, currentScroll);
        Background.transform.localScale = new Vector3(PanelScroll, PanelScroll, PanelScroll);
    }

    public static void SetPlayButton(bool value)
    {
        instance.PlayButton.interactable = value;
    }

    private void EnableNodeMenu()
    {
        NodeMenu.SetActive(true);
        MoveMenu(NodeMenu);
    }

    private void EnableCreationMenu()
    {
        CreationMenu.SetActive(true);
        MoveMenu(CreationMenu);
    }

    public static void HideNodeMenu()
    {
        instance.NodeMenu.SetActive(false);
    }

    private void MoveMenu(GameObject menu)
    {
        RectTransform menuTransform = menu.GetComponent<RectTransform>();
        var offset = new Vector3(menuTransform.sizeDelta.x, -menuTransform.sizeDelta.y, 0) / 2;
        menuTransform.anchoredPosition = Input.mousePosition - new Vector3(Screen.width, Screen.height) / 2 + offset;
    }

    public void OnInstantiateNodeButtonClick(GameObject nodePrefab)
    {
        var node = InstantiateNode(nodePrefab);
        if (ConnectionManager.IsConnectionLineActive())
        {
            Connection finish = node.GetComponent<Connection>();
            if (finish != null)
                ConnectionManager.ForceFinishConnection(finish);
        }
    }

    public static GameObject CreateNodeWithText(Vector3 position, string text)
    {
        var node = instance.InstantiateNode(instance.ActionNodePrefab);
        node.GetComponent<Node>().Panel.GetComponentsInChildren<InputField>().First(x => x.name == "ActionDescription").text = text;
        node.GetComponent<RectTransform>().anchoredPosition = position;
        return node;
    }

    public GameObject InstantiateNode(GameObject nodePrefab)
    {
        var nodeObj = Instantiate(nodePrefab, Background.transform);
        var panel = Instantiate(NodePanelPrefab, PanelBackground.transform);
        nodeObj.GetComponent<RectTransform>().anchoredPosition = MousePosition / PanelScroll;
        Node node = nodeObj.GetComponent<Node>();
        QuestButtonColors.ApplyColorsToButton(false, node.GetComponentInChildren<Button>());
        node.InitializeID();
        node.Panel = panel;
        InitializePanelId(node);
        CreationMenu.SetActive(false);
        Nodes[node.Id] = node;
        NodeData[node.Id] = panel.GetComponent<NodeData>();
        return nodeObj;
    }

    public static GameObject InstantiateDefaultActionNode()
    {
        var nodeObj = Instantiate(instance.ActionNodePrefab, instance.Background.transform);
        var panel = Instantiate(instance.NodePanelPrefab, instance.PanelBackground.transform);
        Node node = nodeObj.GetComponent<Node>();
        node.Panel = panel;
        return nodeObj;
    }

    public static GameObject InstantiateRootNode()
    {
        var nodeObj = Instantiate(instance.RootNodePrefab, instance.Background.transform);
        DataManager.instance.BaseCharactersPanel.GetComponent<CharactersPanel>().UpdatePanels();
        Node node = nodeObj.GetComponent<Node>();
        node.Panel = instance.RootPanel;
        InitializePanelId(node);
        Nodes[node.Id] = node;
        return nodeObj;
    }

    public void UnconnectNode()
    {
        ConnectionManager.RemoveAllConnectionsByNode(ConnectionManager.Current);
        HideNodeMenu();
    }

    public static void InitializePanelId(Node node)
    {
        node.Panel.GetComponentsInChildren<Text>().First(x => x.name == "ID").text = node.Id.ToString();
    }

    public void RemoveSelectedNode()
    {
        ConnectionManager.RemoveAllConnectionsByNode(ConnectionManager.Current);
        Nodes.Remove(ConnectionManager.Current.GetComponent<Node>().Id);
        Destroy(ConnectionManager.Current.gameObject);
        HideNodeMenu();
    }

    public void StartBuild()
    {
        RootNode.StartBuild();
    }

    public void DuplicateSelectedNode()
    {
        var nodeObj = Instantiate(ConnectionManager.Current, Background.transform);
        nodeObj.GetComponent<RectTransform>().anchoredPosition += DuplicateOffset;
        Node node = nodeObj.GetComponent<Node>();
        node.Panel = Instantiate(node.Panel, PanelBackground.transform);
        HideNodeMenu();
        Nodes[node.Id] = node;
        NodeData[node.Id] = node.Panel.GetComponent<NodeData>();
    }

    public void SetNodeName(GameObject node)
    {
        var input = node.GetComponentsInChildren<InputField>().First(x => x.name == "NameField").text;
        var id = int.Parse(node.GetComponentsInChildren<Text>().First(x => x.name == "ID").text);

        if (input == "")
            input = "Action";

        //FindObjectsOfType<Node>().First(x => x.Id == id).gameObject.GetComponentsInChildren<Text>().First(x => x.name == "Name").text = input;
        Nodes[id].gameObject.GetComponentsInChildren<Text>().First(x => x.name == "Name").text = input;
        Nodes[id].Name = input;
    }

    public void SaveNode(GameObject node)
    {
        var id = int.Parse(node.GetComponentsInChildren<Text>().First(x => x.name == "ID").text);
        NodeData[id].NodeDescriptionPanel.GetComponent<NodeDescriptionCharactersPanel>().Save(id);
        //FindObjectsOfType<Node>().First(x => x.Id == id).panelMenu.Save(id);
    }

    public void OnMenuPointerEnter() => IsPointerOutMenu = false;
    public void OnMenuPinterExit() => IsPointerOutMenu = true;

    public void OnNodePointerEnter() => IsPointerOutNode = false;
    public void OnNodePinterExit() => IsPointerOutNode = true;
}
