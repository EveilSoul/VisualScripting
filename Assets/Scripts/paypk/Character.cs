using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[System.Serializable]
public class Character
{
    public string Name;
    public List<CharacterProperty> Properties;

    public Character Clone()
    {
        return new Character()
        {
            Name = Name,
            Properties = Properties.Select(x => new CharacterProperty() { Name = x.Name, Value = (string)x.Value.Clone(), Type = x.Type }).ToList()
        };
    }
}

[System.Serializable]
public class WorldStateCharacter
{
    public string Name;
    public List<WorldStateCharacterProperty> Properties;

    public WorldStateCharacter Clone()
    {
        return new WorldStateCharacter()
        {
            Name = Name,
            Properties = Properties.Select(x => new WorldStateCharacterProperty() { Name = x.Name, Values = x.Values.Clone(), Type = x.Type }).ToList()
        };
    }
}

