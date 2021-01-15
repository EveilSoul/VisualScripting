using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connection : MonoBehaviour
{
    public RectTransform Input;
    public RectTransform Output;

    public void OnPointerEnter()
    {
        ConnectionManager.Current = this;
    }

    public void OnPoinerExit()
    {
        //ConnectionManager.Current = null;
    }
}

public struct ConnectionInfo
{
    public Connection StartPoint;
    public Connection FinishPoint;
    public LineRenderer ConnectionLine;
}
