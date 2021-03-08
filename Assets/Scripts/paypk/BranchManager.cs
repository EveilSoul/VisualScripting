using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class BranchManager
{
    public static void AddBranch(int rootID)
    {
        DataManager.instance.Branches.Add(new Branch()
        {
            RootID = rootID,
            Action_Replicas = new List<Action_Replica>()
        });
    }

    public static void RemoveBranch(int rootID)
    {
        DataManager.instance.Branches.Remove(DataManager.instance.Branches.First(x => x.RootID == rootID));
    }

    public static void AddAction_Replica(int rootID, int nodeID, BranchType branchType, string value)
    {
        var ar = new Action_Replica()
        {
            RootID = rootID,
            NodeID = nodeID,
            BranchType = branchType,
            Value = value
        };
        DataManager.instance.Branches.First(x => x.RootID == rootID).Action_Replicas.Add(ar);
    }

    public static void RemoveAction_Replica(int rootID, int nodeID)
    {
        var ac_rep = DataManager.instance.Branches.First(x => x.RootID == rootID).Action_Replicas;
        ac_rep.Remove(ac_rep.First(x => x.RootID == rootID && x.NodeID == nodeID));
    }

    public static void SaveAction_Replica(int rootID, int nodeID, BranchType branchType, string value)
    {
        if (DataManager.instance.Branches.Exists(x => x.RootID == rootID))
        {
            var ac_rep = DataManager.instance.Branches.First(x => x.RootID == rootID).Action_Replicas;
            if (ac_rep.Exists(x => x.NodeID == nodeID))
            {
                RemoveAction_Replica(rootID, nodeID);
                AddAction_Replica(rootID, nodeID, branchType, value);
            }
        }
    }
}

public struct Branch
{
    public int RootID;
    public List<Action_Replica> Action_Replicas;
}

public struct Action_Replica
{
    public int RootID;
    public int NodeID;
    public BranchType BranchType;
    public string Value;
}

public enum BranchType
{
    Action,
    Replica
}


