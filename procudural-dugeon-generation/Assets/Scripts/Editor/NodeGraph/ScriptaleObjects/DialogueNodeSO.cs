using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;


[CreateAssetMenu(fileName = "Dialogue Node", menuName = "ScriptaleObjects/Dungeon Generation/Dialogue Node")]

public class DialogueNodeSO : ScriptableObject
{
     public string id;
    
    public List<string> childDialogueList = new List<string>();
    public List<string> parentDialogueList = new List<string>();
    [HideInInspector] public ActorTypeListSO actorTypeList;
    [HideInInspector] public DialogueNodeGraphSO dialogueNodeGraph;
    [HideInInspector] public Dictionary<string, ActorTypeSO> roomNodeTypeDictionary;

    public ActorTypeSO actorType;

    #region Editor code

#if UNITY_EDITOR

    [HideInInspector] public Rect rect;
    [HideInInspector] public bool isLeftClikDragging = false;
    [HideInInspector] public bool isSelected = false;

    public void Initialize(Rect rect, DialogueNodeGraphSO dialogueNodeGraph, ActorTypeSO actorType)
    {
        this.rect = rect;
        this.id = Guid.NewGuid().ToString();
        this.name = "Room Node";
        this.dialogueNodeGraph = dialogueNodeGraph;
        this.actorType = actorType;

        actorTypeList = GameResources.Instance.actorTypeList;
    }

    public bool AddChildDialogueNodeToDialogueNode(string childID)
    {
        if (IsChildRoomValid(childID))
        {
            childDialogueList.Add(childID);
            return true;
        }

        return false;
    }

    public bool IsChildRoomValid(string childID)
    {
        if (dialogueNodeGraph.GetDialogueNode(childID).actorType.isNone)
            return false;
        if (id == childID)
            return false;
        if (childDialogueList.Contains(childID))
            return false;
        if (parentDialogueList.Contains(childID))
            return false;
        
        return true;
    }

    public bool AddParentDialogueNodeToDialogueNode(string parentID)
    {
        if(IsParentDialogueValid(parentID))
        {
            parentDialogueList.Add(parentID);
            return true;
        }

        return false;
    }

    public bool RemoveChildDialogueNodeIDFromNode(string childID)
    {
        if (childDialogueList.Contains(childID))
        {
            childDialogueList.Remove(childID);
            return true;
        }

        return false;
    }
    
    public bool RemoveParentDialogueNodeIDFromNode(string parentID)
    {
        if (childDialogueList.Contains(parentID))
        {
            childDialogueList.Remove(parentID);
            return true;
        }

        return false;
    }

    public bool IsParentDialogueValid(string parentID)
    {
        if (parentDialogueList.Contains(parentID))
            return false;
        return true;
    }

    public void Draw(GUIStyle nodeStyle)
    {
        GUILayout.BeginArea(rect, nodeStyle);
        
        EditorGUI.BeginChangeCheck();

        if (parentDialogueList.Count > 0 || actorType.isEnterenceRoom)
        {
            EditorGUILayout.LabelField(actorType.roomNodeTypeName);
        }
        else
        {
            var sellected = actorTypeList.actorNodeTypeList.FindIndex(x => x == actorType);
            var sellection = EditorGUILayout.Popup("", sellected, GetDialogueNodeTypeToDisplay());

            actorType = actorTypeList.actorNodeTypeList[sellection];
        }

        if(EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(this);
        
        GUILayout.EndArea();
    }

    private string[] GetDialogueNodeTypeToDisplay()
    {
        var localRoomNodeTypeList = actorTypeList.actorNodeTypeList;
        var roomArray = new string[localRoomNodeTypeList.Count];

        for (int i = 0; i < localRoomNodeTypeList.Count; i++)
        {
            if (localRoomNodeTypeList[i].disableInNodeGraphEditor)
            {
                roomArray[i] = localRoomNodeTypeList[i].roomNodeTypeName;
            }
        }

        return roomArray;
    }

    public void ProcessEvent(Event currentEvent)
    {
        switch (currentEvent.type)
        {
            case EventType.MouseDown:
                ProcessMouseDownEvent(currentEvent);
                break;
            
            case EventType.MouseDrag:
                ProcessMouseDragEvent(currentEvent);
                break;
            
            case EventType.MouseUp:
                ProcessMouseUpEvent(currentEvent);
                break;
            
            default:
                break;
            
        }
    }

    private void ProcessMouseDownEvent(Event currentEvent)
    {
        if (currentEvent.button == 0)
        {
            ProcessLeftClickDownEvent();
        }
        else if (currentEvent.button == 1)
        {
            ProcessRightClickDownEvent(currentEvent);
        }
    }

    private void ProcessRightClickDownEvent(Event currentEvent)
    {
        dialogueNodeGraph.SetNodeToDrawConnectionLineFrom(node:this, position: currentEvent.mousePosition );
    }

    private void ProcessLeftClickDownEvent()
    {
        Selection.activeObject = this;

        isSelected = !isSelected;
    }
    
    private void ProcessMouseDragEvent(Event currentEvent)
    {
        if (currentEvent.button == 0)
        {
            ProcessLeftClickDragEvent(currentEvent);
        }
    }

    private void ProcessLeftClickDragEvent(Event currenEvent)
    {
        isLeftClikDragging = true;
        
        DragNode(currenEvent.delta);
        GUI.changed = true;
    }

    public void DragNode(Vector2 delta)
    {
        rect.position += delta;
        EditorUtility.SetDirty(this);
    }
    
    private void ProcessMouseUpEvent(Event currentEvent)
    {
        if (currentEvent.button == 0)
        {
            ProcessLeftClickUpEvent();
        }
    }

    private void ProcessLeftClickUpEvent()
    {
        if (isLeftClikDragging)
        {
            isLeftClikDragging = false;
        }
    }

#endif

    #endregion
}
