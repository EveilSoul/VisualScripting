using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DataStorage : MonoBehaviour
{
    public InputField DiagramNameField;
    public Dropdown LoadingDropdown;

    private static DataStorage instance;
    private NodesData Data;
    private Dictionary<int, GameObject> loadedNodes;

    public delegate void ClearingDelegate();
    public static event ClearingDelegate OnSceneClearing;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Data = DiagramInitializer.CreateDefaultDiargam();
            SetActiveLoadingMenu(false);
        }
        else Destroy(gameObject);
    }

    private void Start()
    {
        ClearAll();
        GraphController.InstantiateRootNode();
    }

    private void SetActiveLoadingMenu(bool value)
    {
        LoadingDropdown.transform.parent.gameObject.SetActive(value);
    }

    [ContextMenu("Save")]
    public void OnSaveButtonClick()
    {
        string name = instance.DiagramNameField.text;
        if (name == "")
            name = "UntitledDiagram";
        Save(name);
    }

    public void OnLoadButtonClick()
    {
        var files = Directory.GetFiles(Application.persistentDataPath);
        LoadingDropdown.options = files.Select(x => GetFileName(x)).ToList();
        SetActiveLoadingMenu(true);
    }

    public void OnConfirmLoading()
    {
        ClearAll();
        RootNode.OnLoadingStart();
        Load(LoadingDropdown.options[LoadingDropdown.value].text);
        SetActiveLoadingMenu(false);
    }

    private static Dropdown.OptionData GetFileName(string x)
    {
        var path = x.Split(Path.DirectorySeparatorChar);
        return new Dropdown.OptionData(path.Last());
    }

    public static void Save(string name)
    {
        PrepareNodes(name);
        PrepareDataManager();
        string jsonData = JsonUtility.ToJson(instance.Data);
        File.WriteAllText(Application.persistentDataPath + Path.DirectorySeparatorChar + instance.Data.DiagramName, jsonData);
    }

    private static void PrepareDataManager()
    {
        instance.Data.Characters = DataManager.instance.Characters;
        instance.Data.NodeData = DataManager.instance.Nodes;
        instance.Data.Properties = DataManager.instance.Properties
            .Select(x => new Storage_Property() { Name = x.Key, Type = x.Value.Type, CustomValues = x.Value.CustomValues })
            .ToList();
    }


    private static void PrepareNodes(string name)
    {
        instance.Data = DiagramInitializer.CreateDefaultDiargam();
        instance.Data.DiagramName = name;
        GraphController.AddNodes(instance.Data);
        ConnectionManager.AddConnectionInfo(instance.Data);
    }

    public void ClearAll()
    {
        OnSceneClearing?.Invoke();
        Data = DiagramInitializer.CreateDefaultDiargam();
        foreach (var node in GraphController.Nodes.Values)
        {
            if (node.GetComponent<RootNode>() == null)
                Destroy(node.gameObject);
        }
        ConnectionManager.RemoveAll();
    }

    public static void Load(string name)
    {
        if (File.Exists(Application.persistentDataPath + Path.DirectorySeparatorChar + name))
        {
            instance.Data = ScriptableObject.CreateInstance<NodesData>();
            string jsonStats = File.ReadAllText(Application.persistentDataPath + Path.DirectorySeparatorChar + name);
            JsonUtility.FromJsonOverwrite(jsonStats, instance.Data);
            GraphController.Nodes = new Dictionary<int, Node>();
            GraphController.NodeData = new Dictionary<int, NodeData>();
            PlaceNodes();
            AddConnections();
            InitializeDataManager();
        }
    }

    private static void InitializeDataManager()
    {
        DataManager.instance.Characters = instance.Data.Characters;
        DataManager.instance.Nodes = instance.Data.NodeData;

        DataManager.instance.Properties = new Dictionary<string, Property>();
        instance.Data.Properties.ForEach(x => DataManager.instance.Properties[x.Name] = new Property() { Type = x.Type, CustomValues = x.CustomValues });
    }

    private static void AddConnections()
    {
        foreach (var info in instance.Data.Connections)
        {
            var line = ConnectionManager.CreateConnectionLine();
            Connection start = instance.loadedNodes[info.StartNodeId].GetComponent<Connection>();
            var startPosition = ConnectionManager.GetOutputPosition(start);
            Connection finish = instance.loadedNodes[info.FinishNodeId].GetComponent<Connection>();
            var finishPosition = ConnectionManager.GetInputPosition(finish);
            line.SetPositions(new[] { startPosition, finishPosition });
            ConnectionManager.AddConnection(start, finish, line);
        }
    }

    public static void PlaceNodes()
    {
        instance.loadedNodes = new Dictionary<int, GameObject>();
        foreach (var info in instance.Data.Nodes)
        {
            if (info.IsRootNode)
            {
                var nodeObject = GraphController.InstantiateRootNode();
                nodeObject.transform.position = info.Position;
                var node = nodeObject.GetComponent<RootNode>();
                node.Id = info.Id;
                instance.loadedNodes[info.Id] = nodeObject;
            }
            else
            {
                var nodeObject = GraphController.InstantiateDefaultActionNode();
                nodeObject.transform.position = info.Position;
                var node = nodeObject.GetComponent<Node>();
                node.Id = info.Id;
                GraphController.InitializePanelId(node);
                GraphController.Nodes[node.Id] = node;
                GraphController.NodeData[node.Id] = node.Panel.GetComponent<NodeData>();
                instance.loadedNodes[info.Id] = nodeObject;
            }
        }
    }
}
