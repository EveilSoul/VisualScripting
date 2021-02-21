using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class Node : MonoBehaviour, IPointerClickHandler
{
    public int Id;
    public GameObject Panel;
    public NodeDescriptionCharactersPanel panelMenu;

    public string Name;

    public List<Character> WorldState;

    private Connection nodeConnection;

    protected void Start()
    {
        nodeConnection = gameObject.GetComponent<Connection>();
    }

    public void InitializeID()
    {
        Name = "Action";
        Id = GenerateID();
    }

    public static int GenerateID()
    {
        var id = PlayerPrefs.GetInt("NodeId", 1);
        PlayerPrefs.SetInt("NodeId", id + 1);
        return id;
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount == 2)
        {
            Panel.SetActive(true);
            if (panelMenu == null && GetComponent<RootNode>() == null)
                panelMenu = Panel.GetComponent<NodeData>().NodeDescriptionPanel.GetComponent<NodeDescriptionCharactersPanel>();
            panelMenu?.OpenExistNode(Id);
        }

        GraphController.OnNodePointerClick(this);
    }

    public void RecursiveBuild()
    {
        if (!ConnectionManager.ConnectionDictionary.ContainsKey(nodeConnection))
            return;

        foreach (var connection in ConnectionManager.ConnectionDictionary[nodeConnection].Where(x => x.StartPoint == nodeConnection))
        {
            connection.FinishPoint.Node.WorldState = WorldState.Clone();
            if (connection.FinishPoint.Node.CheckCondition())
            {
                connection.FinishPoint.Node.ApplyEffects();
                connection.FinishPoint.Node.RecursiveBuild();
            }
            else
            {
                connection.FinishPoint.Node.OnErrorOccurred();
            }
        }
    }

    public void ApplyEffects()
    {
        var nodeData = DataManager.instance.Nodes.First(x => x.Id == Id);
        var effects = nodeData.Effect;

        foreach (var character in effects.NodeCharacters)
        {
            var worldStateCharacter = WorldState.First(x => x.Name == character.Name);
            foreach (var effectProperty in character.PropertyValues)
            {
                CharacterProperty worldStateProperty = worldStateCharacter.Properties.First(x => x.Name == effectProperty.Name);
                if(effectProperty.PropertyType == PropertyType.Int)
                {
                    var operand = effectProperty.Value[0];
                    var effectValue = int.Parse(effectProperty.Value.Substring(1, effectProperty.Value.Length - 1));
                    var worldValue = int.Parse(worldStateProperty.Value);

                    switch (operand)
                    {
                        case '=':
                            worldValue = effectValue;
                            break;
                        case '+':
                            worldValue += effectValue;
                            break;
                        case '-':
                            worldValue -= effectValue;
                            break;
                    }

                    worldStateProperty.Value = worldValue.ToString();
                }
                else worldStateProperty.Value = effectProperty.Value;
            }
        }
    }

    public void OnErrorOccurred()
    {
        var outline = gameObject.GetComponent<UnityEngine.UI.Outline>();
        outline.enabled = true;
        outline.effectColor = GraphController.ErrorOutlineColor;
    }

    public bool CheckCondition()
    {
        var nodeData = DataManager.instance.Nodes.First(x => x.Id == Id);
        var condition = nodeData.Condition;
        if (WorldState != null)
        {
            

            foreach (var character in condition.NodeCharacters)
            {
                var worldStateCharacter = WorldState.First(x => x.Name == character.Name);
                foreach (var conditionProperty in character.PropertyValues)
                {
                    CharacterProperty worldStateProperty = worldStateCharacter.Properties.First(x => x.Name == conditionProperty.Name);
                    if (conditionProperty.PropertyType == PropertyType.Int)
                    {
                        var operand = conditionProperty.Value[0];
                        var conditionValue = int.Parse(conditionProperty.Value.Substring(1, conditionProperty.Value.Length - 1));
                        var worldValue = int.Parse(worldStateProperty.Value);

                        bool isSuccess = true;

                        switch (operand)
                        {
                            case '=':
                                if (worldValue != conditionValue)
                                    isSuccess = false;
                                break;
                            case '>':
                                if (worldValue <= conditionValue)
                                    isSuccess = false;
                                break;
                            case '<':
                                if (worldValue >= conditionValue)
                                    isSuccess = false;
                                break;
                        }

                        if (!isSuccess)
                        {
                            Debug.LogError($"Condition error at {ToString()}. " +
                                $"Character: {character.Name}, " +
                                $"Property {worldStateProperty.Name}. " +
                                $"Expected: real value {operand} {conditionValue}," +
                                $"but real value is {worldValue}");
                            return false;
                        }
                    }
                    else if (worldStateProperty.Value != conditionProperty.Value)
                    {
                        Debug.LogError($"Condition error at {ToString()}. " +
                            $"Character: {character.Name}, " +
                            $"Property {worldStateProperty.Name}. " +
                            $"Expected  value: {conditionProperty.Value}. " +
                            $"Real value: {worldStateProperty.Value}");
                        return false;
                    }
                }
            }
        }
        else
        {
            Debug.LogError($"World State is null at {ToString()}");
            return false;
        }
        return true;
    }
}
