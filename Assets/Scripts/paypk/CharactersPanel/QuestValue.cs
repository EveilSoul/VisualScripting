using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

class QuestValue : MonoBehaviour
{
    public int Index { get; set; }
    public InputField Name;
    public QuestPanel Quest_Panel { get; set; }

    public void Remove()
    {
        Quest_Panel.Remove(Index);
    }
}

