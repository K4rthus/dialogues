using System.Collections.Generic;
using UnityEngine;

public class FlagManager : MonoBehaviour
{
    public static FlagManager Instance { get; private set; }

    private Dictionary<string, bool> flags = new Dictionary<string, bool>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public bool CheckFlag(string flagName)
    {
        return flags.TryGetValue(flagName, out bool value) && value;
    }

    public void SetFlag(string flagName, bool state)
    {
        flags[flagName] = state;
    }
}