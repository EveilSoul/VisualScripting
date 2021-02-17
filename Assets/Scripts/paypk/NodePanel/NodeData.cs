using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NodeData : MonoBehaviour
{
    public static NodeData instance;

    public GameObject NodeDescriptionPanel;
    public GameObject CharactersPanel;
    public GameObject ConditionPanel;
    public GameObject NodeCharactersPanel;
    public GameObject EffectsPanel;
    public GameObject EffectsNodeCharactersPanel;

    private void Awake()
    {
        instance = this;
    }

    public NodeData GetMyNodeData(ref int ID, Transform transform)
    {
        if (ID == -1)
        {
            var p = transform.parent;
            while (p.tag != DataManager.NodePanelTag)
                p = p.parent;
            ID = int.Parse(p.GetChild(0).GetComponent<Text>().text);
        }
        return GraphController.NodeData[ID];
    }
}
