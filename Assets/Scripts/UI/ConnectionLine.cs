using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ConnectionLine : MonoBehaviour, IPointerClickHandler
{
    private LineRenderer line;

    private void Start()
    {
        line = gameObject.GetComponent<LineRenderer>();
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        ConnectionManager.OnLineSelected(line);
    }
}
