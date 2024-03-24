using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Dialogue Node", menuName = "ScriptaleObjects/Dungeon Generation/Room Node Graph")]
public class DialogueNodeGraphSO : ScriptableObject
{
    [HideInInspector] public ActorNodeTypeSO actorNodeType;
    [HideInInspector] public List<DialogueNodeSO> roomNodeList = new List<DialogueNodeSO>();
    [HideInInspector] public Dictionary<string, DialogueNodeSO> roomNodeDictionary = new Dictionary<string, DialogueNodeSO>();


    #region Editor

#if UNITY_EDITOR
    
    [FormerlySerializedAs("actorNodeToDrawLineFrom")] [FormerlySerializedAs("roomNodeToDrawLineFrom")] [HideInInspector] public DialogueNodeSO dialogueNodeToDrawLineFrom = null;
    [HideInInspector] public Vector2 linePos;

    private void Awake()
    {
        LoadRoomNodeDictionary();
    }

    public void LoadRoomNodeDictionary()
    {
        roomNodeDictionary.Clear();
        
        foreach (var node in roomNodeList)
        {
            roomNodeDictionary[node.id] = node;
        }   
    }

    public DialogueNodeSO GetRoomNode(string roomNodeID)
    {
        if (roomNodeDictionary.TryGetValue(roomNodeID, out DialogueNodeSO roomNode))
        {
            return roomNode;
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
        LoadRoomNodeDictionary();
    }

#endif

    #endregion
}
