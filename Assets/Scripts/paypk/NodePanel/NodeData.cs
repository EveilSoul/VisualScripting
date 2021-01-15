using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
