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

        ClearPanels();

        foreach (var p in DataManager.instance.Properties)
        {
            var panel = Instantiate(Panel, Parent);
            var val = panel.GetComponent<PropertiesListProperty>();
            val.Name.text = p.Key;
            val.Type.text = ToStr(p.Value.Type);
            panels.Add(panel);
        }

        SetDynamicSize();
    }

    private void SetDynamicSize()
    {
        var transformParent = Parent.GetComponent<RectTransform>();
        var cellSize = Parent.GetComponent<GridLayoutGroup>().cellSize.y;
        transformParent.sizeDelta = new Vector2(transformParent.sizeDelta.x, Math.Max(472, panels.Count * cellSize));
        transformParent.anchoredPosition = new Vector2(0, -transformParent.sizeDelta.y / 2);
    }

    private void ClearPanels()
    {
        foreach (var p in panels)
        {
            Destroy(p);
        }
        panels = new List<GameObject>();
    }

    public string ToStr(PropertyType propertyType)
    {
        switch (propertyType)
        {
            case PropertyType.Bool:
                return "Логический";
            case PropertyType.Int:
                return "Целочисленный";
            default:
                return "Пользовательский";
        }
    }
}

