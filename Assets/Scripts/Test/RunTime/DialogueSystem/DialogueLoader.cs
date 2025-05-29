using System;
using TMPro;
using UnityEngine;

public class DialogueLoader : MonoBehaviour
{
    #region Variables

    [SerializeField] private TextMeshProUGUI dialogueTxt;
    [SerializeField] private Transform dialogueContainer;

    private DialogueNodeGraphSO dialogueNodeGraph;
    private DialogueNodeSO dialogueNode;
    
    public static Action<string> OnCallDialogueData;
    public static Action OnDialogueEnd;

    #endregion

    #region Callbacks

    private void OnEnable()
    {
        OnCallDialogueData += GetNextDialogueText;
    }

    private void OnDisable()
    {
        OnCallDialogueData -= GetNextDialogueText;
    }

    #endregion

    #region OtherMethods

    private void LoadDialogueData(string dialogueId)
    {
        dialogueNodeGraph = DialogueDataManager.Instance.GetDialogue(dialogueId);
        dialogueNode = dialogueNodeGraph.dialogueNodeList[0];
        Debug.Log("dialogue node:" + dialogueNodeGraph + " dialogueNode: " + dialogueNode);

        var textbox = InstantiateTextBox(dialogueNode.actorType.actorName, dialogueNode.dialogueText);
        textbox.name = dialogueNode.id;
    }

    private void GetNextDialogueText(string dialogueId)
    {
        var childDialogueList = dialogueNodeGraph.dialogueNodeDictionary[dialogueId].childDialogueList;

        if (childDialogueList.Count <= 0)
        {
            OnDialogueEnd?.Invoke();
            return;
        }
        
        for (int i = 0; i < childDialogueList.Count; i++)
        {
            var nextDialogue = dialogueNodeGraph.dialogueNodeDictionary[childDialogueList[i]];
            var textbox = InstantiateTextBox(nextDialogue.actorType.actorName, nextDialogue.dialogueText);
            textbox.name = childDialogueList[i];
        }
    }

    private TextMeshProUGUI InstantiateTextBox(string actorName, string dialogueText)
    {
        TextMeshProUGUI textBox = Instantiate(dialogueTxt, dialogueContainer);
        textBox.text = actorName + ": " + dialogueText;
        return textBox;
    }

    private TextMeshProUGUI InstantiateTextBox(string dialogueText)
    {
        TextMeshProUGUI textBox = Instantiate(dialogueTxt, dialogueContainer);
        textBox.text =  dialogueText;
        return textBox;
    }

    #endregion
}
