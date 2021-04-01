using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AllCharactersPanel : MonoBehaviour
{
    public int ID = -1;

    public GameObject CharacterPanel;
    public Transform Parent;

    private List<GameObject> panels = new List<GameObject>();

    private void OnEnable()
    {
        ClearPanels();

        var characters = NodeData.GetMyNodeData(ref ID, transform).NodeDescriptionPanel.GetComponent<NodeDescriptionCharactersPanel>().GetNotSelectedCharacters();
        
        foreach (var p in characters)
        {
            var panel = Instantiate(CharacterPanel, Parent);
            panels.Add(panel);
            panel.GetComponent<CharacterData>().SetName(p);
        }
        SetDynamicSize();
    }

    private void SetDynamicSize()
    {
        var transformParent = Parent.GetComponent<RectTransform>();
        var cellSize = Parent.GetComponent<GridLayoutGroup>().cellSize.y;
        int count = panels.Count;
        var countInTheRow = 6;
        var r = (count / countInTheRow + count % countInTheRow - 3) * (cellSize + 40) + 25;
        transformParent.sizeDelta = new Vector2(transformParent.sizeDelta.x, Math.Max(410, r));
        transformParent.anchoredPosition = new Vector2(0, -transformParent.sizeDelta.y / 2);
    }

    public void ClearPanels()
    {
        foreach (var p in panels)
        {
            Destroy(p);
        }
        panels = new List<GameObject>();
    }
}
