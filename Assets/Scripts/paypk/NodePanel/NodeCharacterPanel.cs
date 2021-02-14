using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeCharacterPanel : MonoBehaviour
{
    public int ID = -1;

    public GameObject CharacterPanel;
    public Transform Parent;

    private List<GameObject> panels = new List<GameObject>();

    private void OnEnable()
    {
        ClearPanels();

        var characters = NodeData.instance.GetMyNodeData(ref ID, transform).NodeDescriptionPanel.GetComponent<NodeDescriptionCharactersPanel>().GetSelectedCharactersToCondition();

        if (characters == null)
            return;

        foreach (var p in characters)
        {
            var panel = Instantiate(CharacterPanel, Parent);
            panels.Add(panel);
            panel.GetComponent<NodeCharacterData>().SetName(p);
        }
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
