using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueLoader : MonoBehaviour
{
    #region Variables

    [SerializeField] private TextMeshProUGUI dialogueTxt;
    [SerializeField] private TextMeshProUGUI actorNameTxt;
    [SerializeField] private Image actorImg;
    [SerializeField] private GameObject TextBoxParentObject;

    private DialogueNodeGraphSO dialogueNodeGraph;
    private DialogueNodeSO dialogueNode;
    private List<TextMeshProUGUI> activeDialogueBoxes = new List<TextMeshProUGUI>();
    private int dialogueBoxIndex = 0;
    
    public static Action<string> OnCallDialogueData;
    public static Action OnDialogueEnd;
    public static Action<ActionTypes, int> OnCallDialogueAction;

    #endregion

    #region Callbacks

    private void OnEnable()
    {
        OnCallDialogueData += GetNextDialogueText;
        OnCallDialogueData += CallDialogueAction;
    }

    private void OnDisable()
    {
        OnCallDialogueData -= GetNextDialogueText;
        OnCallDialogueData -= CallDialogueAction;
    }

    #endregion

    #region OtherMethods

    public void LoadDialogueData(string dialogueId)
    {
        dialogueNodeGraph = DialogueDataManager.Instance.GetDialogue(dialogueId);
        dialogueNode = dialogueNodeGraph.dialogueNodeList[0];
        Debug.Log("dialogue node:" + dialogueNodeGraph + " dialogueNode: " + dialogueNode);

        var textbox = InstantiateTextBox(dialogueNode.actorType.actorName, dialogueNode.dialogueText, dialogueNode.actorType.actorIcon);
        textbox.name = dialogueNode.id;
    }

    private void GetNextDialogueText(string dialogueId)
    {
        ResetDialogueBoxes();
        
        var childDialogueList = dialogueNodeGraph.dialogueNodeDictionary[dialogueId].childDialogueList;

        if (childDialogueList.Count <= 0)
        {
            OnDialogueEnd?.Invoke();
            return;
        }
        
        for (int i = 0; i < childDialogueList.Count; i++)
        {
            var nextDialogue = dialogueNodeGraph.dialogueNodeDictionary[childDialogueList[i]];
            var textbox = InstantiateTextBox(nextDialogue.actorType.actorName, nextDialogue.dialogueText, nextDialogue.actorType.actorIcon);
            textbox.name = childDialogueList[i];
        }
    }

    private void CallDialogueAction(string dialogueId)
    {
        var currentActions = dialogueNodeGraph.dialogueNodeDictionary[dialogueId].actions;

        for (int i = 0; i < currentActions.Count; i++)
        {
            OnCallDialogueAction?.Invoke(currentActions[i].actionType, currentActions[i].ActionValue);
        }
    }

    private TextMeshProUGUI InstantiateTextBox(string actorName, string dialogueText, Sprite actorIcon)
    {
        TextMeshProUGUI textBox;
        
        if (activeDialogueBoxes.Count > dialogueBoxIndex)
        {
            activeDialogueBoxes[dialogueBoxIndex].gameObject.SetActive(true);
            textBox = activeDialogueBoxes[dialogueBoxIndex];
        }
        else
        {
            textBox = Instantiate(dialogueTxt, TextBoxParentObject.transform, true);
            activeDialogueBoxes.Add(textBox);
        }
        
        dialogueBoxIndex++;
        
        textBox.text = dialogueText;
        actorNameTxt.text = actorName;
        actorImg.sprite = actorIcon;
        return textBox;
    }

    private void ResetDialogueBoxes()
    {
        for (int i = 0; i < dialogueBoxIndex; i++)
        {
            activeDialogueBoxes[i].gameObject.SetActive(false);
        }

        dialogueBoxIndex = 0;
    }

    #endregion
}
