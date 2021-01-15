using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Node : MonoBehaviour, IPointerClickHandler
{
    public int Id;
    public GameObject Panel;
    public NodeDescriptionCharactersPanel panelMenu;

    public void InitializeID()
    {
        Id = GenerateID();
    }

    public static int GenerateID()
    {
        var id = PlayerPrefs.GetInt("NodeId", 1);
        PlayerPrefs.SetInt("NodeId", id + 1);
        return id;
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount == 2)
        {
            Panel.SetActive(true);
            if (panelMenu == null && GetComponent<RootNode>() == null)
                panelMenu = Panel.GetComponent<NodeData>().NodeDescriptionPanel.GetComponent<NodeDescriptionCharactersPanel>();
            panelMenu?.OpenExistNode(Id);
        }

        GraphController.OnNodePointerClick(this);
    }
}
