using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RootNode : Node
{
    private static RootNode instance;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        if (instance == null)
        {
            instance = this;
            if (Id < 1)
                Id = GenerateID();
            Name = "Root node";
        }
        else Destroy(gameObject);

        Panel.SetActive(false);
    }

    public static void StartBuild()
    {
        instance.WorldState = DataManager.instance.Characters.Clone();
        instance.RecursiveBuild();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public static void OnLoadingStart()
    {
        if (instance != null)
            Destroy(instance.gameObject);
        instance = null;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        if (eventData.clickCount == 2)
        {
            DataManager.instance.BaseCharactersPanel.GetComponent<CharactersPanel>().UpdatePanels();
        }
    }
}
