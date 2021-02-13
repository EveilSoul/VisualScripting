using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GraphController : MonoBehaviour
{
    public Color NormalColor;
    public static Color NormalOutlineColor => instance.NormalColor;
    public Color ErrorColor;
    public static Color ErrorOutlineColor => instance.ErrorColor;

    private static GraphController instance;

    public static Vector3 MousePosition => Input.mousePosition - new Vector3(Screen.width, Screen.height) / 2;

    public GameObject ActionNodePrefab;
    public GameObject RootNodePrefab;
    public GameObject Background;
    public GameObject PanelBackground;
    public GameObject NodePanelPrefab;
    public GameObject RootPanel;

    public GameObject CreationMenu;
    public GameObject NodeMenu;

    public Vector2 DuplicateOffset;

    private bool canCloseCreationMenu;
    private bool IsPointerOutNode = true;

    public static Dictionary<int, Node> Nodes;

    private Outline selectedNode;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        CreationMenu.SetActive(false);
        NodeMenu.SetActive(false);
        DataStorage.OnSceneClearing += OnSceneClearing;
        Nodes = new Dictionary<int, Node>();
    }

    [ContextMenu("Reset Nodes ID")]
    public void ResetNodesId()
    {
        PlayerPrefs.SetInt("NodeId", 1);
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

    public static void AddNodes(NodesData data)
    {
        //foreach (var node in FindObjectsOfType<Node>())
        foreach (var node in Nodes.Values)
            data.Nodes.Add(new Storage_NodeInfo() { Id = node.Id, Position = node.transform.position, IsRootNode = node.GetComponent<RootNode>() != null });
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

        if (CreationMenu.activeInHierarchy && (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(0) && canCloseCreationMenu))
        {
            CreationMenu.SetActive(false);
        }

        if (NodeMenu.activeInHierarchy && (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(0) && canCloseCreationMenu))
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
        menuTransform.anchoredPosition = MousePosition + offset;
    }

    public void OnInstantiateNodeButtonClick(GameObject nodePrefab) => InstantiateNode(nodePrefab);


    public GameObject InstantiateNode(GameObject nodePrefab)
    {
        var nodeObj = Instantiate(nodePrefab, Background.transform);
        var panel = Instantiate(NodePanelPrefab, PanelBackground.transform);
        nodeObj.GetComponent<RectTransform>().anchoredPosition = MousePosition;
        Node node = nodeObj.GetComponent<Node>();
        node.InitializeID();
        node.Panel = panel;
        InitializePanelId(node);
        CreationMenu.SetActive(false);
        Nodes[node.Id] = node;
        return nodeObj;
    }

    public static GameObject InstantiateDefaultActionNode()
    {
        var nodeObj = Instantiate(instance.ActionNodePrefab, instance.Background.transform);
        var panel = Instantiate(instance.NodePanelPrefab, instance.PanelBackground.transform);
        Node node = nodeObj.GetComponent<Node>();
        node.Panel = panel;
        Nodes[node.Id] = node;
        return nodeObj;
    }

    public static GameObject InstantiateRootNode()
    {
        var nodeObj = Instantiate(instance.RootNodePrefab, instance.Background.transform);
        Node node = nodeObj.GetComponent<Node>();
        node.Panel = instance.RootPanel;
        InitializePanelId(node);
        Nodes[node.Id] = node;
        return nodeObj;
    }

    public static void InitializePanelId(Node node)
    {
        node.Panel.GetComponentsInChildren<Text>().First(x => x.name == "ID").text = node.Id.ToString();
    }

    public void RemoveSelectedNode()
    {
        ConnectionManager.RemoveAllConnectionsByNode(ConnectionManager.Current);
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
    }

    public void SetNodeName(GameObject node)
    {
        var input = node.GetComponentsInChildren<InputField>().First(x => x.name == "NameField").text;
        var id = int.Parse(node.GetComponentsInChildren<Text>().First(x => x.name == "ID").text);

        if (input == "")
            input = "Action";

        //FindObjectsOfType<Node>().First(x => x.Id == id).gameObject.GetComponentsInChildren<Text>().First(x => x.name == "Name").text = input;
        Nodes.Values.First(x => x.Id == id).gameObject.GetComponentsInChildren<Text>().First(x => x.name == "Name").text = input;
    }

    public void SaveNode(GameObject node)
    { 
        var id = int.Parse(node.GetComponentsInChildren<Text>().First(x => x.name == "ID").text);
        instance.NodePanelPrefab.GetComponent<NodeData>().NodeDescriptionPanel.GetComponent<NodeDescriptionCharactersPanel>().Save(id);
        //FindObjectsOfType<Node>().First(x => x.Id == id).panelMenu.Save(id);
    }

    public void OnMenuPointerEnter() => instance.canCloseCreationMenu = false;
    public void OnMenuPinterExit() => instance.canCloseCreationMenu = true;

    public void OnNodePointerEnter() => instance.IsPointerOutNode = false;
    public void OnNodePinterExit() => instance.IsPointerOutNode = true;
}
