using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class PropertiesListProperty : MonoBehaviour
{
    public Text Name;
    public Text Type;

    public void OnDeleteClick()
    {
        DataManager.instance.Properties.Remove(Name.text);
        DataManager.instance.OnDeletePropertyInvoke(Name.text);
        Destroy(gameObject);
    }

    public void OnEditClick()
    {
        DataManager.instance.PropertiesListPanel.SetActive(false);
        DataManager.instance.PropertyEditPanel.SetActive(true);
        DataManager.instance.PropertyEditPanel.GetComponent<PropertyEditPanel>().SetName(Name.text);
    }
}



