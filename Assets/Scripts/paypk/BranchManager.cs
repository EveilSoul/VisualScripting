using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class BranchManager
{
    private static void AddBranch(int nodeID, BranchType branchType, string value)
    {
        var branch = new Branch()
        {
            NodeID = nodeID,
            BranchType = branchType,
            Value = value
        };
        DataManager.instance.Branches.Add(branch);
    }

    private static void RemoveBranch(int nodeID)
    {
        DataManager.instance.Branches.Remove(DataManager.instance.Branches.First(x => x.NodeID == nodeID));
    }

    public static void SaveBranch(int nodeID, BranchType branchType, string value)
    {
        if (DataManager.instance.Branches.Exists(x => x.NodeID == nodeID))
            RemoveBranch(nodeID);

        if (value != "")
        {
            AddBranch(nodeID, branchType, value);
        }
    }
}

[Serializable]
public struct Branch
{
    public int NodeID;
    public BranchType BranchType;
    public string Value;
}

public enum BranchType
{
    Action,
    Replica
}


