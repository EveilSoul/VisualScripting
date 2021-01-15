using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Nodes", fileName = "NodesData")]
public class NodesData : ScriptableObject
{
    public string DiagramName;
    public List<Storage_NodeInfo> Nodes;
    public List<Storage_Connection> Connections;
}

public static class DiagramInitializer
{
    public static NodesData CreateDefaultDiargam()
    {
        var result = ScriptableObject.CreateInstance<NodesData>();
        result.DiagramName = "Untitled diagram";
        result.Nodes = new List<Storage_NodeInfo>();
        result.Connections = new List<Storage_Connection>();
        return result;
    }
}

[System.Serializable]
public struct Storage_NodeInfo
{
    public bool IsRootNode;
    public int Id;
    public Vector3 Position;
}

[System.Serializable]
public struct Storage_Connection
{
    public int StartNodeId;
    public int FinishNodeId;
}
