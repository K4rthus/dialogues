using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject optionButtonPrefab;

    [Header("UI Elements")]
    public TextMeshProUGUI dialogueText;
    public Transform optionsPanel;

    [Header("Dialogue File")]
    public string dialogueFileName;

    private DialogueNode currentNode;
    private DialogueData currentDialogue;

    private void Start()
    {
        if (PlayerPrefs.HasKey("DialogueFile"))
        {
            string file = PlayerPrefs.GetString("DialogueFile");
            string node = PlayerPrefs.GetString("TargetNode");
            PlayerPrefs.DeleteKey("DialogueFile");
            PlayerPrefs.DeleteKey("TargetNode");
            LoadDialogue(file);
            StartDialogue(node);
        }
        else
        {
            LoadDialogue(dialogueFileName);
        }
    }

    public void LoadDialogue(string fileName)
    {
        TextAsset jsonData = Resources.Load<TextAsset>(fileName);
        currentDialogue = JsonUtility.FromJson<DialogueData>(jsonData.text);
        StartDialogue(currentDialogue.startNode);
    }

    void StartDialogue(string nodeId)
    {
        currentNode = currentDialogue.nodes.Find(n => n.id == nodeId);
        UpdateDialogueUI();
    }

    void UpdateDialogueUI()
    {
        dialogueText.text = currentNode.text;

        foreach (Transform child in optionsPanel)
            Destroy(child.gameObject);

        Debug.Log($"Доступно опций: {currentNode.options.Count}");

        foreach (var option in currentNode.options)
        {
            bool conditionsMet = CheckConditions(option.conditions);
            Debug.Log($"Опция '{option.text}': conditionsMet={conditionsMet}");

            if (conditionsMet)
            {
                GameObject button = Instantiate(optionButtonPrefab, optionsPanel);
                button.GetComponentInChildren<TextMeshProUGUI>().text = option.text;
                button.GetComponent<Button>().onClick.AddListener(() => SelectOption(option));
            }
        }
    }

    bool CheckConditions(List<FlagCondition> conditions)
    {
        foreach (var condition in conditions)
        {
            bool requiredState = condition.requiredState;
            bool actualState = FlagManager.Instance.CheckFlag(condition.flagName);

            if (actualState != requiredState)
            {
                return false;
            }
        }
        return true;
    }

    void ApplyOperations(List<FlagOperation> operations)
    {
        foreach (var op in operations)
        {
            FlagManager.Instance.SetFlag(op.flagName, op.newState);
        }
    }

    public void SelectOption(DialogueOption option)
    {
        ApplyOperations(option.flagOperations);

        if (option.sceneTransitions.Count > 0)
        {
            foreach (var transition in option.sceneTransitions)
            {
                Debug.Log($"Переход: {transition.unitySceneName}");
                LoadNewScene(transition.unitySceneName, transition.targetScene, transition.targetNode);
                return;
            }
        }

        if (!string.IsNullOrEmpty(option.targetNode))
        {
            StartDialogue(option.targetNode);
        }
        else
        {
            CloseDialogue();
        }
    }

    private void LoadNewScene(string unitySceneName, string dialogueFile, string targetNode)
    {
        PlayerPrefs.SetString("DialogueFile", dialogueFile);
        PlayerPrefs.SetString("TargetNode", targetNode);
        SceneManager.LoadScene(unitySceneName);
    }

    private void CloseDialogue()
    {
        gameObject.SetActive(false);
    }
}