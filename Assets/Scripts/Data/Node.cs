using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public int Id;

    public void InitializeID()
    {
        Id = GenerateID();
    }

    public static int GenerateID()
    {
        var id = PlayerPrefs.GetInt("NodeId", 1);
        PlayerPrefs.SetInt("NodeId", id + 1);
        return id;
    }
}
