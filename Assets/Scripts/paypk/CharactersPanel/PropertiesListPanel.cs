using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class PropertiesListPanel : MonoBehaviour
{
    public GameObject Panel;
    public Transform Parent;

    private List<GameObject> panels = new List<GameObject>();

    private void OnEnable()
    {
        Debug.Log("Start");
        ClearPanels();

        foreach (var p in DataManager.instance.Properties)
        {
            var panel = Instantiate(Panel, Parent);
            var val = panel.GetComponent<PropertiesListProperty>();
            val.Name.text = p.Key;
            val.Type.text = p.Value.Type.ToString();
            panels.Add(panel);
        }
    }

    private void ClearPanels()
    {
        foreach (var p in panels)
        {
            Destroy(p);
        }
        panels = new List<GameObject>();
    }
}

