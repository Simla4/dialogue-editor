using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Dialogue Node", menuName = "ScriptaleObjects/Dungeon Generation/Dialogue Node Graph")]
public class DialogueNodeGraphSO : ScriptableObject
{
    [HideInInspector] public ActorTypeSO actorType;
    [HideInInspector] public List<DialogueNodeSO> dialogueNodeList = new List<DialogueNodeSO>();
    [HideInInspector] public Dictionary<string, DialogueNodeSO> dialogueNodeDictionary = new Dictionary<string, DialogueNodeSO>();


    #region Editor

#if UNITY_EDITOR
    
    [HideInInspector] public DialogueNodeSO dialogueNodeToDrawLineFrom = null;
    [HideInInspector] public Vector2 linePos;

    private void Awake()
    {
        LoadDialogueNodeDictionary();
    }

    public void LoadDialogueNodeDictionary()
    {
        dialogueNodeDictionary.Clear();
        
        foreach (var node in dialogueNodeList)
        {
            dialogueNodeDictionary[node.id] = node;
        }   
    }

    /// <summary>
    /// Returns which dialogue node it is according to the id we provide.
    /// </summary>
    /// <param name="dialogueNodeID"></param>
    /// <returns></returns>
    public DialogueNodeSO GetDialogueNode(string dialogueNodeID)
    {
        if (dialogueNodeDictionary.TryGetValue(dialogueNodeID, out DialogueNodeSO dialogueNode))
        {
            return dialogueNode;
        }
        
        return null;
    }

    public void SetNodeToDrawConnectionLineFrom(Vector2 position, DialogueNodeSO node)
    {
        dialogueNodeToDrawLineFrom = node;
        linePos = position;
    }

    public void OnValidate()
    {
        LoadDialogueNodeDictionary();
    }

#endif

    #endregion
}
