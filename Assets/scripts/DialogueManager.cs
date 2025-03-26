using System.Collections.Generic;
using UnityEngine;
using TMPro; // Не забудьте подключить TextMeshPro
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject optionButtonPrefab;

    [Header("UI Elements")]
    public TMP_Text dialogueText; // Используем TextMeshPro
    public Transform optionsPanel;

    [Header("Dialogue File")]
    public string dialogueFileName; // Имя файла без расширения

    private DialogueNode currentNode;
    private DialogueData currentDialogue;

    void Start()
    {
        LoadDialogue(dialogueFileName); // Используем имя файла
    }

    public void LoadDialogue(string fileName)
    {
        TextAsset jsonData = Resources.Load<TextAsset>(fileName);
        if (jsonData != null)
        {
            currentDialogue = JsonUtility.FromJson<DialogueData>(jsonData.text);
            StartDialogue(currentDialogue.startNode);
        }
        else
        {
            Debug.LogError("Не удалось загрузить файл: " + fileName);
        }
    }

    public void StartDialogue(string nodeId)
    {
        currentNode = currentDialogue.nodes.Find(n => n.id == nodeId);
        UpdateDialogueUI();
    }

    void UpdateDialogueUI()
    {
        dialogueText.text = currentNode.text;

        foreach (Transform child in optionsPanel)
            Destroy(child.gameObject);

        foreach (var option in currentNode.options)
        {
            if (CheckConditions(option.conditions))
            {
                GameObject button = Instantiate(optionButtonPrefab, optionsPanel);
                button.GetComponentInChildren<TMP_Text>().text = option.text;
                button.GetComponent<Button>().onClick.AddListener(() => SelectOption(option));
            }
        }
    }

    bool CheckConditions(List<FlagCondition> conditions)
    {
        foreach (var condition in conditions)
            if (FlagManager.Instance.CheckFlag(condition.flagName) != condition.requiredState)
                return false;
        return true;
    }

    void ApplyFlagOperations(List<FlagOperation> operations)
    {
        foreach (var op in operations)
            FlagManager.Instance.SetFlag(op.flagName, op.newState);
    }

    public void SelectOption(DialogueOption option)
    {
        ApplyFlagOperations(option.flagOperations);
        StartDialogue(option.targetNode);
    }
}