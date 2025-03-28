using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueOption
{
    [TextArea(1, 3)] public string text;
    public string targetNode;
    public List<SceneTransition> sceneTransitions = new();
    public List<FlagCondition> conditions = new();
    public List<FlagOperation> flagOperations = new();
}