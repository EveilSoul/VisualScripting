using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootNode : Node
{
    private static RootNode instance;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
            if (Id < 1)
                Id = GenerateID();
        }
        else Destroy(gameObject);
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
}
