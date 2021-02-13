using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tools
{
    
}

public static class ListExtentions
{
    public static List<T> Clone<T>(this List<T> list)
    {
        var result = new List<T>(list.Count);
        list.ForEach(x => result.Add(x));
        return result;
    }

    public static List<Character> Clone(this List<Character> list)
    {
        var result = new List<Character>(list.Count);
        list.ForEach(x => result.Add(x.Clone()));
        return result;
    }
}
