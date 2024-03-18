using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Room Node", menuName = "ScriptaleObjects/Dungeon Generation/Room Node Graph")]
public class ActorNodeGraphSO : ScriptableObject
{
    [FormerlySerializedAs("roomNodeType")] [HideInInspector] public ActorNodeTypeSO actorNodeType;
    [HideInInspector] public List<ActorNodeSO> roomNodeList = new List<ActorNodeSO>();
    [HideInInspector] public Dictionary<string, ActorNodeSO> roomNodeDictionary = new Dictionary<string, ActorNodeSO>();


    #region Editor

#if UNITY_EDITOR
    
    [FormerlySerializedAs("roomNodeToDrawLineFrom")] [HideInInspector] public ActorNodeSO actorNodeToDrawLineFrom = null;
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

    public ActorNodeSO GetRoomNode(string roomNodeID)
    {
        if (roomNodeDictionary.TryGetValue(roomNodeID, out ActorNodeSO roomNode))
        {
            return roomNode;
        }
        
        return null;
    }

    public void SetNodeToDrawConnectionLineFrom(Vector2 position, ActorNodeSO node)
    {
        actorNodeToDrawLineFrom = node;
        linePos = position;
    }

    public void OnValidate()
    {
        LoadRoomNodeDictionary();
    }

#endif

    #endregion
}
