using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Node : MonoBehaviour, IPointerClickHandler
{
    public int Id;
    public GameObject Panel;

    private NodeDescriptionCharactersPanel panelMenu;
    public NodeDescriptionCharactersPanel PanelMenu
    {
        get
        {
            if (panelMenu == null && GetComponent<RootNode>() == null)
                panelMenu = Panel.GetComponent<NodeData>().NodeDescriptionPanel.GetComponent<NodeDescriptionCharactersPanel>();
            return panelMenu;
        }
    }

    public string Name;

    public List<Character> WorldState;

    private Connection nodeConnection;

    protected void Start()
    {
        nodeConnection = gameObject.GetComponent<Connection>();
    }

    public void OnQuestButtonClick(Button button)
    {
        // Выключаем все ненужное
        var br = Panel.GetComponentsInChildren<BranchDescription>(true).First();
        br.ID = Id;
        br.Button = button;
        br.gameObject.SetActive(true);
        // Обычная процедура активации панели из ноды
        Panel.SetActive(true);
        PanelMenu?.OpenExistNode(Id);

        Panel.GetComponentsInChildren<NodeDescriptionCharactersPanel>(true).First(x => x.name == "NodeDescriptionn-Panel").gameObject.SetActive(false);
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
            PanelMenu?.OpenExistNode(Id);
            //if (Id != RootNode.instance.Id)
            //    Debug.Log(GraphController.GetAllNodeTextById(Id));
        }

        GraphController.OnNodePointerClick(this);
        GraphController.InitializePanelId(this);
    }

    public void OnErrorOccurred()
    {
        var outline = gameObject.GetComponent<UnityEngine.UI.Outline>();
        outline.enabled = true;
        outline.effectColor = GraphController.ErrorOutlineColor;
    }

    public void StartRecursiveBuild()
    {
        if (!ConnectionManager.ConnectionDictionary.ContainsKey(nodeConnection) || ConnectionManager.ConnectionDictionary[nodeConnection].Where(x => x.StartPoint == nodeConnection).Count() == 0)
            return;

        List<Connection> successChilds = new List<Connection>();

        foreach (var connection in ConnectionManager.ConnectionDictionary[nodeConnection].Where(x => x.StartPoint == nodeConnection))
        {
            connection.FinishPoint.Node.WorldState = WorldState.Clone();
            if (connection.FinishPoint.Node.CheckCondition())
            {
                successChilds.Add(connection.FinishPoint);
            }
        }

        if(successChilds.Count == 0)
        {
            OnErrorOccurred();
            LogError($"Состояние мира в одной из ветвей в действии {Id} не проходит условия ни одного из потомков");
        }
        else if (successChilds.Count > 1 && successChilds.Any(x => x.Node.Panel.GetComponentInChildren<BranchDescription>(true).Text.text == ""))
        {
            OnErrorOccurred();
            LogError($"Действие {Id} имеет несколько потомков с подходящими услвовиями, однако не везде определены действия/реплики для перехода");

            foreach (var ch in successChilds)
                ch.Node.CheckCondition();
        }

        foreach (var child in successChilds)
        {
            child.Node.ApplyEffects();
            child.Node.StartRecursiveBuild();
        }
    }

    public void ApplyEffects()
    {
        var nodeData = DataManager.instance.Nodes.FirstOrDefault(x => x.Id == Id);
        var effects = nodeData.Effect;

        if (effects.NodeCharacters == null)
            return;

        foreach (var character in effects.NodeCharacters)
        {
            var worldStateCharacter = WorldState.FirstOrDefault(x => x.Name == character.Name);

            if (worldStateCharacter == null)
            {
                var newChar = new Character() { Name = character.Name, Properties = character.PropertyValues.Select(p => new CharacterProperty() { Name = p.Name, Type = p.PropertyType, Value = p.Value }).ToList() };
                worldStateCharacter = newChar;
                WorldState.Add(newChar);
            }

            foreach (var effectProperty in character.PropertyValues)
            {
                var worldStateProperty = worldStateCharacter.Properties.FirstOrDefault(x => x.Name == effectProperty.Name);

                if (worldStateProperty == null)
                {
                    worldStateProperty = new CharacterProperty();
                    worldStateProperty.Name = effectProperty.Name;
                    worldStateProperty.Type = effectProperty.PropertyType;

                    if (effectProperty.PropertyType == PropertyType.Int)
                        worldStateProperty.Value = "0";


                    if (worldStateCharacter.Properties == null)
                        worldStateCharacter.Properties = new List<CharacterProperty>();
                    worldStateCharacter.Properties.Add(worldStateProperty);
                }

                if (effectProperty.PropertyType == PropertyType.Int)
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
                else
                {
                    worldStateProperty.Value = effectProperty.Value;
                }
            }
        }
    }

    public bool CheckCondition()
    {
        var nodeData = DataManager.instance.Nodes.FirstOrDefault(x => x.Id == Id);
        var condition = nodeData.Condition;
        if (condition.NodeCharacters == null)
            return true;
        if (WorldState != null)
        {
            foreach (var character in condition.NodeCharacters)
            {
                var worldStateCharacter = WorldState.First(x => x.Name == character.Name);
                foreach (var conditionProperty in character.PropertyValues)
                {
                    var worldStateProperty = worldStateCharacter.Properties.FirstOrDefault(x => x.Name == conditionProperty.Name);
                    if (worldStateProperty == null)
                    {
                        LogError($"character {character.Name} has no property named {conditionProperty.Name}");
                        return false;
                    }
                    if (conditionProperty.PropertyType == PropertyType.Int)
                    {
                        var operand = conditionProperty.Value[0];
                        var conditionValue = int.Parse(conditionProperty.Value.Substring(1, conditionProperty.Value.Length - 1));
                        var worldValues = int.Parse(worldStateProperty.Value);

                        bool isSuccess = true;

                        switch (operand)
                        {
                            case '=':
                                if (worldValues != conditionValue)
                                    isSuccess = false;
                                break;
                            case '>':
                                if (worldValues <= conditionValue)
                                    isSuccess = false;
                                break;
                            case '<':
                                if (worldValues >= conditionValue)
                                    isSuccess = false;
                                break;
                        }

                        if (!isSuccess)
                        {
                            //LogError($"Condition error at {ToString()}. " +
                            //    $"Character: {character.Name}, " +
                            //    $"Property {worldStateProperty.Name}. " +
                            //    $"Expected: real value {operand} {conditionValue}," +
                            //    $"but real value is {worldValues}");
                            return false;
                        }
                    }
                    else if (worldStateProperty.Value != conditionProperty.Value)
                    {
                        //LogError($"Condition error at {ToString()}. " +
                        //    $"Character: {character.Name}, " +
                        //    $"Property {worldStateProperty.Name}. " +
                        //    $"Expected  value: {conditionProperty.Value}. " +
                        //    $"Real value: {worldStateProperty.Value}");
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

    private void LogError(string message)
    {
        Debug.LogError(message);
    }


    //public void ApplyEffectsBuild()
    //{
    //    if (!ConnectionManager.ConnectionDictionary.ContainsKey(nodeConnection))
    //        return;

    //    foreach (var connection in ConnectionManager.ConnectionDictionary[nodeConnection].Where(x => x.StartPoint == nodeConnection))
    //    {
    //        if (connection.FinishPoint.Node.WorldState == null)
    //            connection.FinishPoint.Node.WorldState = WorldState.Clone();
    //        else
    //        {
    //            var state = connection.FinishPoint.Node.WorldState;
    //            foreach (var character in WorldState.Clone())
    //            {
    //                var properties = state.FirstOrDefault(x => x.Name == character.Name)?.Properties;

    //                foreach (var property in character.Properties)
    //                {
    //                    if (properties != null && properties.Exists(x => x.Name == property.Name))
    //                    {
    //                        var findedProperty = properties.First(x => x.Name == property.Name);
    //                        findedProperty.Values.AddRange(property.Values);
    //                        findedProperty.Values = findedProperty.Values.Distinct().ToList();
    //                    }
    //                    else
    //                    {
    //                        if (!state.Exists(x => x.Name == character.Name))
    //                        {
    //                            state.Add(character);
    //                            continue;
    //                        }
    //                        else if (properties == null)
    //                        {
    //                            WorldStateCharacter worldCharacter = state.First(x => x.Name == character.Name);
    //                            worldCharacter.Properties = new List<WorldStateCharacterProperty>();
    //                            properties = worldCharacter.Properties;
    //                        }
    //                        properties.Add(property);
    //                    }
    //                }
    //            }
    //        }

    //        connection.FinishPoint.Node.ApplyEffects();
    //        connection.FinishPoint.Node.ApplyEffectsBuild();
    //    }
    //}

    //public void CheckConditionBuild()
    //{
    //    if (!ConnectionManager.ConnectionDictionary.ContainsKey(nodeConnection))
    //        return;

    //    foreach (var connection in ConnectionManager.ConnectionDictionary[nodeConnection].Where(x => x.StartPoint == nodeConnection))
    //    {
    //        if (connection.FinishPoint.Node.CheckCondition())
    //        {
    //            connection.FinishPoint.Node.CheckConditionBuild();
    //        }
    //        else
    //        {
    //            connection.FinishPoint.Node.OnErrorOccurred();
    //            RootNode.IsBuldSuccess = false;
    //        }
    //    }
    //}

    //public void RecursiveBuild()
    //{
    //    if (!ConnectionManager.ConnectionDictionary.ContainsKey(nodeConnection))
    //        return;

    //    foreach (var connection in ConnectionManager.ConnectionDictionary[nodeConnection].Where(x => x.StartPoint == nodeConnection))
    //    {
    //        connection.FinishPoint.Node.WorldState = WorldState.Clone();
    //        if (connection.FinishPoint.Node.CheckCondition())
    //        {
    //            connection.FinishPoint.Node.ApplyEffects();
    //            connection.FinishPoint.Node.RecursiveBuild();
    //        }
    //        else
    //        {
    //            connection.FinishPoint.Node.OnErrorOccurred();
    //            RootNode.IsBuldSuccess = false;
    //        }
    //    }
    //}

    //public void ApplyEffects()
    //{
    //    var nodeData = DataManager.instance.Nodes.FirstOrDefault(x => x.Id == Id);
    //    var effects = nodeData.Effect;

    //    if (effects.NodeCharacters == null)
    //        return;

    //    foreach (var character in effects.NodeCharacters)
    //    {
    //        var worldStateCharacter = WorldState.FirstOrDefault(x => x.Name == character.Name);

    //        if (worldStateCharacter == null)
    //        {
    //            var newChar = new WorldStateCharacter() { Name = character.Name, Properties = character.PropertyValues.Select(p => new WorldStateCharacterProperty() { Name = p.Name, Type = p.PropertyType, Values = new List<string>() { p.Value } }).ToList() };
    //            worldStateCharacter = newChar;
    //            WorldState.Add(newChar);
    //        }

    //        foreach (var effectProperty in character.PropertyValues)
    //        {
    //            var worldStateProperty = worldStateCharacter.Properties.FirstOrDefault(x => x.Name == effectProperty.Name);

    //            if (worldStateProperty == null)
    //            {
    //                worldStateProperty = new WorldStateCharacterProperty();
    //                worldStateProperty.Name = effectProperty.Name;
    //                worldStateProperty.Type = effectProperty.PropertyType;

    //                if (effectProperty.PropertyType == PropertyType.Int)
    //                    worldStateProperty.Values = new List<string>() { "0" };


    //                if (worldStateCharacter.Properties == null)
    //                    worldStateCharacter.Properties = new List<WorldStateCharacterProperty>();
    //                worldStateCharacter.Properties.Add(worldStateProperty);
    //            }

    //            if (effectProperty.PropertyType == PropertyType.Int)
    //            {
    //                var operand = effectProperty.Value[0];
    //                var effectValue = int.Parse(effectProperty.Value.Substring(1, effectProperty.Value.Length - 1));
    //                var worldValues = worldStateProperty.Values.Select(x => int.Parse(x)).ToList();

    //                switch (operand)
    //                {
    //                    case '=':
    //                        for (int i = 0; i < worldValues.Count; i++)
    //                        {
    //                            worldValues[i] = effectValue;
    //                        }
    //                        break;
    //                    case '+':
    //                        for (int i = 0; i < worldValues.Count; i++)
    //                        {
    //                            worldValues[i] += effectValue;
    //                        }
    //                        break;
    //                    case '-':
    //                        for (int i = 0; i < worldValues.Count; i++)
    //                        {
    //                            worldValues[i] -= effectValue;
    //                        }
    //                        break;
    //                }

    //                worldStateProperty.Values = worldValues.Select(x => x.ToString()).ToList();
    //            }
    //            else
    //            {
    //                if (worldStateProperty.Values == null)
    //                    worldStateProperty.Values = new List<string>();
    //                worldStateProperty.Values.Add(effectProperty.Value);
    //            }
    //        }
    //    }
    //}

    //public bool CheckCondition()
    //{
    //    var nodeData = DataManager.instance.Nodes.FirstOrDefault(x => x.Id == Id);
    //    var condition = nodeData.Condition;
    //    if (condition.NodeCharacters == null)
    //        return true;
    //    if (WorldState != null)
    //    {
    //        foreach (var character in condition.NodeCharacters)
    //        {
    //            var worldStateCharacter = WorldState.First(x => x.Name == character.Name);
    //            foreach (var conditionProperty in character.PropertyValues)
    //            {
    //                var worldStateProperty = worldStateCharacter.Properties.FirstOrDefault(x => x.Name == conditionProperty.Name);
    //                if (worldStateProperty == null)
    //                {
    //                    LogError($"character {character.Name} has no property named {conditionProperty.Name}");
    //                    return false;
    //                }
    //                if (conditionProperty.PropertyType == PropertyType.Int)
    //                {
    //                    var operand = conditionProperty.Value[0];
    //                    var conditionValue = int.Parse(conditionProperty.Value.Substring(1, conditionProperty.Value.Length - 1));
    //                    var worldValues = worldStateProperty.Value.Select(x => int.Parse(x));

    //                    bool isSuccess = true;

    //                    switch (operand)
    //                    {
    //                        case '=':
    //                            if (worldValues.All(x => x != conditionValue))
    //                                isSuccess = false;
    //                            break;
    //                        case '>':
    //                            if (worldValues.All(x => x <= conditionValue))
    //                                isSuccess = false;
    //                            break;
    //                        case '<':
    //                            if (worldValues.All(x => x >= conditionValue))
    //                                isSuccess = false;
    //                            break;
    //                    }

    //                    if (!isSuccess)
    //                    {
    //                        LogError($"Condition error at {ToString()}. " +
    //                            $"Character: {character.Name}, " +
    //                            $"Property {worldStateProperty.Name}. " +
    //                            $"Expected: real value {operand} {conditionValue}," +
    //                            $"but real values is {worldValues.Select(x => x.ToString()).Aggregate((x, y) => x + " " + y)}");
    //                        return false;
    //                    }
    //                }
    //                else if (worldStateProperty.Values.All(x => x != conditionProperty.Value))
    //                {
    //                    LogError($"Condition error at {ToString()}. " +
    //                        $"Character: {character.Name}, " +
    //                        $"Property {worldStateProperty.Name}. " +
    //                        $"Expected  value: {conditionProperty.Value}. " +
    //                        $"Real value: {worldStateProperty.Values.Aggregate((x, y) => x + " " + y)}");
    //                    return false;
    //                }
    //            }
    //        }
    //    }
    //    else
    //    {
    //        Debug.LogError($"World State is null at {ToString()}");
    //        return false;
    //    }
    //    return true;
    //}
}
