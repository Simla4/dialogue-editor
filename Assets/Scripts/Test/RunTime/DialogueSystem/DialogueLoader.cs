using System;
using TMPro;
using UnityEngine;

public class DialogueLoader : MonoBehaviour
{
    #region Variables

    [SerializeField] private TextMeshProUGUI dialogueTxt;
    [SerializeField] private Transform dialogueContainer;

    private DialogueNodeGraphSO dialogue;
    
    public static Action OnCallNextDialogueText;

    #endregion

    #region OtherMethods

    public void LoadDialogueData()
    { 
        dialogue = DialogueDataManager.Instance.GetDialogue("FirstDialogue");

        TextMeshProUGUI textBox = InstantiateTextBox();
        textBox.text = dialogue.dialogueNodeList[0].actorType.actorName + ": "+ dialogue.dialogueNodeList[0].dialogueText;
    }

    public void GetNextDialogueText()
    {
        
    }

    private TextMeshProUGUI InstantiateTextBox()
    {
        TextMeshProUGUI textBox = Instantiate(dialogueTxt, dialogueContainer);

        return textBox;
    }

    #endregion
}
