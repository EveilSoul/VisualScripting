using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Property
{
    public PropertyType Type;
    //public bool BoolValue;
    //public int IntValue;
    public List<string> CustomValues;
}

[System.Serializable]
public class CharacterProperty
{
    public string Name;
    public PropertyType Type;
    public string Value;

    public override bool Equals(object obj)
    {
        return this.Name == ((CharacterProperty)obj).Name;
    }
}

public enum PropertyType
{
    Bool,
    Int,
    Custom
}

