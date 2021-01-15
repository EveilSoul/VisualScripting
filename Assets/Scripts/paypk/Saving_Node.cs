using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PropertyValue
{
    public string Name;
    public PropertyType PropertyType;
    public string Value;
}

public struct NodeCharacter
{
    public string Name;
    public List<PropertyValue> PropertyValues;
}

public struct Condition
{
    public List<NodeCharacter> NodeCharacters;
}

public struct Effect
{
    public List<NodeCharacter> NodeCharacters;
}

public struct Saving_Node
{
    public int Id;
    public List<string> CharacterNames;
    public Condition Condition;
    public string Text;
    public Effect Effect;
}


