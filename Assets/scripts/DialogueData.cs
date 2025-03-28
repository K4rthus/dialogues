using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class DialogueData
{
    public string startNode;
    public List<DialogueNode> nodes = new();
}