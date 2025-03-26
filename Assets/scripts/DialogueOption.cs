using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueOption
{
    [TextArea(1, 3)] public string text;
    public string targetNode;
    public List<FlagCondition> conditions = new List<FlagCondition>();
    public List<FlagOperation> flagOperations = new List<FlagOperation>();
}