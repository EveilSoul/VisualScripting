using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphController : MonoBehaviour
{
    public static Vector3 MousePosition => Input.mousePosition - new Vector3(Screen.width, Screen.height) / 2;

    public GameObject NodePrefab;
    public GameObject ConnectionLinePrefab;

    public GameObject Background;

    public GameObject CreationMenu;
    public GameObject NodeMenu;

    public static Connection CurrentConnection;

    private bool canCloseCreationMenu;
    private bool canOpenCreationMenu = true;

    private bool isDrawingLine;

    private LineRenderer currentLine;

    // Start is called before the first frame update
    void Start()
    {
        CreationMenu.SetActive(false);
        NodeMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (canOpenCreationMenu)
                EnableCreationMenu();
            else EnableNodeMenu();
        }

        if (CreationMenu.activeInHierarchy && (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(0) && canCloseCreationMenu))
        {
            CreationMenu.SetActive(false);
        }

        if (currentLine != null)
            currentLine.SetPosition(1, MousePosition);

        if (Input.GetMouseButton(0) && currentLine != null)
            FinishConnection();
    }

    private void EnableNodeMenu()
    {
        NodeMenu.SetActive(true);
        MoveMenu(NodeMenu);
    }   

    public void StartConnection()
    {
        currentLine = Instantiate(ConnectionLinePrefab, Background.transform).GetComponent<LineRenderer>();
        currentLine.SetPositions(new[] { CurrentConnection.Output.position });
    }

    public void FinishConnection()
    {
        currentLine.SetPosition(1, CurrentConnection.Input.position);
        currentLine = null;
        CurrentConnection = null;
    }

    private void EnableCreationMenu()
    {
        CreationMenu.SetActive(true);
        MoveMenu(CreationMenu);
    }

    private void MoveMenu(GameObject menu)
    {
        RectTransform menuTransform = menu.GetComponent<RectTransform>();
        var offset = new Vector3(menuTransform.sizeDelta.x, -menuTransform.sizeDelta.y, 0) / 2;
        menuTransform.anchoredPosition = MousePosition + offset;
    }

    public void InstantiateNode(GameObject nodePrefab)
    {
        var node = Instantiate(nodePrefab, Background.transform);
        node.GetComponent<RectTransform>().anchoredPosition = MousePosition;
        CreationMenu.SetActive(false);
    }

    public void OnMenuPointerEnter() => canCloseCreationMenu = false;
    public void OnMenuPinterExit() => canCloseCreationMenu = true;

    public void OnNodePointerEnter() => canOpenCreationMenu = false;
    public void OnNodePinterExit() => canOpenCreationMenu = true;
}
