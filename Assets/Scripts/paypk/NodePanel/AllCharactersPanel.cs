using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AllCharactersPanel : MonoBehaviour
{
    public GameObject CharacterPanel;
    public Transform Parent;

    private List<GameObject> panels = new List<GameObject>();

    private void OnEnable()
    {
        ClearPanels();

        var characters = NodeData.instance.NodeDescriptionPanel.GetComponent<NodeDescriptionCharactersPanel>().GetNotSelectedCharacters();
        
        foreach (var p in characters)
        {
            var panel = Instantiate(CharacterPanel, Parent);
            panels.Add(panel);
            panel.GetComponent<CharacterData>().SetName(p);
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
