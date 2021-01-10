using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connection : MonoBehaviour
{
    public Transform Input;
    public Transform Output;

    public void OnPointerEnter()
    {
        GraphController.CurrentConnection = this;
    }

    public void OnPoinerExit()
    {
        GraphController.CurrentConnection = null;
    }
}
