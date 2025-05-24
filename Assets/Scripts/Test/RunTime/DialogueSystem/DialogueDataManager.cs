using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueDataManager : MonoSingleton<DialogueDataManager>
{
    #region Variables
    
    private const string DIALOGUE_DATA_FOLDER_NAME = "DialogueData";

    private Dictionary<string, DialogueNodeGraphSO> dialogueDictionary = new();

    #endregion

    #region Callbacks

    private void Start()
    {
        LoadAllData();
    }

    #endregion

    #region Other Methods

    private void LoadAllData()
    {
        DialogueNodeGraphSO[] allDialogueDatas = Resources.LoadAll<DialogueNodeGraphSO>(DIALOGUE_DATA_FOLDER_NAME);
        
        foreach (DialogueNodeGraphSO dialogue in allDialogueDatas)
        {
            if (dialogue != null && !dialogueDictionary.ContainsKey(dialogue.name))
            {
                dialogueDictionary.Add(dialogue.name, dialogue);
            }
        }
    }

    public DialogueNodeGraphSO GetDialogue(string dialogueId)
    {
        if (dialogueDictionary.ContainsKey(dialogueId))
        {
            return dialogueDictionary[dialogueId];
        }
        else
        {
            Debug.LogError("DialogueDataManager not found dialogue with id: " + dialogueId);
            return null;
        }
    }

    #endregion
}
