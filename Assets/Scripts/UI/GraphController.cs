using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphController : MonoBehaviour
{
    private static GraphController instance;

    public static Vector3 MousePosition => Input.mousePosition - new Vector3(Screen.width, Screen.height) / 2;

    public GameObject ActionNodePrefab;
    public GameObject RootNodePrefab;
    public GameObject Background;

    public GameObject CreationMenu;
    public GameObject NodeMenu;

    public Vector2 DuplicateOffset;

    private bool canCloseCreationMenu;
    private bool IsPointerOutNode = true;
    private bool isOneClick;

    private Outline selectedNode;

    [ContextMenu("Reset Nodes ID")]
    public void ResetNodesId()
    {
        PlayerPrefs.SetInt("NodeId", 1);
    }

    public static void AddNodes(NodesData data)
    {
        foreach (var node in FindObjectsOfType<Node>())
            data.Nodes.Add(new Storage_NodeInfo() { Id = node.Id, Position = node.transform.position, IsRootNode = node.GetComponent<RootNode>() != null });
    }

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        CreationMenu.SetActive(false);
        NodeMenu.SetActive(false);
        DataStorage.OnSceneClearing += OnSceneClearing;
    }

    public void OnSceneClearing()
    {
        isOneClick = false;
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

        if (Input.GetMouseButtonDown(0) && !IsPointerOutNode)
        {
            isOneClick = !isOneClick;
            if (!isOneClick)
            {
                selectedNode.enabled = false;
            }
            if (isOneClick || ConnectionManager.Current.gameObject != selectedNode.gameObject)
            {
                if (selectedNode != null)
                {
                    selectedNode.enabled = false;
                    if (ConnectionManager.Current.gameObject != selectedNode.gameObject)
                    {
                        isOneClick = true;
                    }
                }
                selectedNode = ConnectionManager.Current.GetComponent<Outline>();
                selectedNode.enabled = true;
            }
        }
        else if (Input.GetMouseButtonDown(0))
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
        var node = Instantiate(nodePrefab, Background.transform);
        node.GetComponent<RectTransform>().anchoredPosition = MousePosition;
        node.GetComponent<Node>().InitializeID();
        CreationMenu.SetActive(false);
        return node;
    }

    public static GameObject InstantiateDefaultActionNode()
    {
        var node = Instantiate(instance.ActionNodePrefab, instance.Background.transform);
        return node;
    }

    public static GameObject InstantiateRootNode()
    {
        var node = Instantiate(instance.RootNodePrefab, instance.Background.transform);
        return node;
    }

    public void RemoveSelectedNode()
    {
        ConnectionManager.RemoveAllConnectionsByNode(ConnectionManager.Current);
        Destroy(ConnectionManager.Current.gameObject);
        HideNodeMenu();
    }

    public void DuplicateSelectedNode()
    {
        var node = Instantiate(ConnectionManager.Current, Background.transform);
        node.GetComponent<RectTransform>().anchoredPosition += DuplicateOffset;
        HideNodeMenu();
    }

    public void OnMenuPointerEnter() => instance.canCloseCreationMenu = false;
    public void OnMenuPinterExit() => instance.canCloseCreationMenu = true;

    public void OnNodePointerEnter() => instance.IsPointerOutNode = false;
    public void OnNodePinterExit() => instance.IsPointerOutNode = true;
}
