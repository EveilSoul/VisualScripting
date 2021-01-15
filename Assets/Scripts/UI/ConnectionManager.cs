using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionManager : MonoBehaviour
{
    public static ConnectionManager instance;

    public static Connection StartPoint;
    public static Connection Current;

    public GameObject ConnectionLinePrefab;
    public GameObject Background;

    private bool isDrawingLine;

    private LineRenderer currentLine;

    private List<ConnectionInfo> AllConnections;
    private Dictionary<Connection, HashSet<ConnectionInfo>> Connections;
    private Vector3 offset;

    private void Start()
    {
        instance = this;
        Connections = new Dictionary<Connection, HashSet<ConnectionInfo>>();
        AllConnections = new List<ConnectionInfo>();
    }

    public static void AddConnectionInfo(NodesData data)
    {
        foreach(var connection in instance.AllConnections)
        {
            int startId = connection.StartPoint.GetComponent<Node>().Id;
            int finishId = connection.FinishPoint.GetComponent<Node>().Id;
            data.Connections.Add(new Storage_Connection() { StartNodeId = startId, FinishNodeId = finishId });
        }
    }

    private void Update()
    {
        if (currentLine != null)
            currentLine.SetPosition(1, GraphController.MousePosition - offset);

        if (Input.GetMouseButton(0) && currentLine != null)
            FinishConnection();

        if (Input.GetKeyDown(KeyCode.Escape) && currentLine != null)
        {
            Destroy(currentLine);
            currentLine = null;
        }
    }

    public void StartConnection()
    {
        StartPoint = Current;
        offset = new Vector3(StartPoint.GetComponent<RectTransform>().sizeDelta.x / 2, 0, 0);
        currentLine = CreateConnectionLine();
        currentLine.SetPositions(new[] { GetOutputPosition(StartPoint), Vector3.zero });

        GraphController.HideNodeMenu();
    }

    public static LineRenderer CreateConnectionLine()
    {
        var line = Instantiate(instance.ConnectionLinePrefab, instance.Background.transform).GetComponent<LineRenderer>();
        line.positionCount = 2;
        return line;
    }

    public static void OnNodeMove(Connection node)
    {
        if (!instance.Connections.ContainsKey(node))
            return;

        foreach (var info in instance.Connections[node])
        {
            if (info.StartPoint == node)
            {
                info.ConnectionLine.SetPosition(0, GetOutputPosition(node));
            }
            else
            {
                info.ConnectionLine.SetPosition(1, GetInputPosition(node));
            }
        }
    }

    public static Vector3 GetOutputPosition(Connection node)
    {
        var position = (Vector3)node.Output.anchoredPosition;
        position.y += node.Output.sizeDelta.y / 2;
        position += (Vector3)node.Output.parent.GetComponent<RectTransform>().anchoredPosition;
        return position;
    }

    public static Vector3 GetInputPosition(Connection node)
    {
        var position = (Vector3)node.Input.anchoredPosition;
        position.y += node.Input.sizeDelta.y / 2;
        position += (Vector3)node.Input.parent.GetComponent<RectTransform>().anchoredPosition;
        position.x -= node.Input.parent.GetComponent<RectTransform>().sizeDelta.x;
        return position;
    }

    public static void RemoveAll()
    {
        if (instance == null || instance.AllConnections == null)
            return;

        foreach(var connection in instance.AllConnections)
        {
            Destroy(connection.ConnectionLine);
        }
        instance.Connections = new Dictionary<Connection, HashSet<ConnectionInfo>>();
    }

    public static void RemoveAllConnectionsByNode(Connection node)
    {
        if (!instance.Connections.ContainsKey(node))
            return;

        foreach (var info in instance.Connections[node])
        {
            Destroy(info.ConnectionLine);

            if (info.StartPoint != node)
                instance.Connections[info.StartPoint].Remove(info);
            else instance.Connections[info.FinishPoint].Remove(info);
            instance.AllConnections.Remove(info);
        }
        instance.Connections.Remove(node);
    }

    public void FinishConnection()
    {
        if (Current.Output == null)
        {
            Destroy(currentLine);
            currentLine = null;
            return;
        }

        currentLine.SetPosition(1, GetInputPosition(Current));
        SaveConnection(StartPoint, Current, currentLine);
        currentLine = null;
    }

    private void SaveConnection(Connection startPoint, Connection finishPoint, LineRenderer connection)
    {
        if (AllConnections == null)
            AllConnections = new List<ConnectionInfo>();
        var info = new ConnectionInfo() { StartPoint = startPoint, FinishPoint = finishPoint, ConnectionLine = connection };
        AllConnections.Add(info);

        if (!Connections.ContainsKey(info.StartPoint))
            Connections[info.StartPoint] = new HashSet<ConnectionInfo>();
        Connections[info.StartPoint].Add(info);

        if (!Connections.ContainsKey(info.FinishPoint))
            Connections[info.FinishPoint] = new HashSet<ConnectionInfo>();
        Connections[info.FinishPoint].Add(info);
    }

    public static void AddConnection(Connection startPoint, Connection finishPoint, LineRenderer connection)
    {
        instance.SaveConnection(startPoint, finishPoint, connection);
    }
}
