using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;


[CreateAssetMenu(fileName = "Room Node", menuName = "ScriptaleObjects/Dungeon Generation/Room Node")]

public class RoomNodeSO : ScriptableObject
{
     public string id;
    
     public List<string> childRoomList = new List<string>();
     public List<string> parentRoomList = new List<string>();
    [HideInInspector] public ActorTypeListSO actorTypeList;
    [HideInInspector] public RoomNodeGraphSO roomNodeGraph;
    [HideInInspector] public Dictionary<string, RoomNodeTypeSO> roomNodeTypeDictionary;

    public RoomNodeTypeSO roomNodeType;

    #region Editor code

#if UNITY_EDITOR

    [HideInInspector] public Rect rect;
    [HideInInspector] public bool isLeftClikDragging = false;
    [HideInInspector] public bool isSelected = false;

    public void Initialize(Rect rect, RoomNodeGraphSO roomNodeGraph, RoomNodeTypeSO roomNodeType)
    {
        this.rect = rect;
        this.id = Guid.NewGuid().ToString();
        this.name = "Room Node";
        this.roomNodeGraph = roomNodeGraph;
        this.roomNodeType = roomNodeType;

        actorTypeList = GameResources.Instance.actorTypeList;
    }

    public bool AddChildRoomNodeToRoomNoode(string childID)
    {
        if (IsChildRoomValid(childID))
        {
            childRoomList.Add(childID);
            return true;
        }

        return false;
    }

    public bool IsChildRoomValid(string childID)
    {
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isNone)
            return false;
        if (id == childID)
            return false;
        if (childRoomList.Contains(childID))
            return false;
        if (parentRoomList.Contains(childID))
            return false;
        
        return true;
    }

    public bool AddParentRoomNodeToRoomNode(string parentID)
    {
        if(IsParentRoomValid(parentID))
        {
            parentRoomList.Add(parentID);
            return true;
        }

        return false;
    }

    public bool RemoveChildRoomNodeIDFromNode(string childID)
    {
        if (childRoomList.Contains(childID))
        {
            childRoomList.Remove(childID);
            return true;
        }

        return false;
    }
    
    public bool RemoveParentRoomNodeIDFromNode(string parentID)
    {
        if (childRoomList.Contains(parentID))
        {
            childRoomList.Remove(parentID);
            return true;
        }

        return false;
    }

    public bool IsParentRoomValid(string parentID)
    {
        if (parentRoomList.Contains(parentID))
            return false;
        return true;
    }

    public void Draw(GUIStyle nodeStyle)
    {
        GUILayout.BeginArea(rect, nodeStyle);
        
        EditorGUI.BeginChangeCheck();

        if (parentRoomList.Count > 0 || roomNodeType.isEnterenceRoom)
        {
            EditorGUILayout.LabelField(roomNodeType.roomNodeTypeName);
        }
        else
        {
            var sellected = actorTypeList.roomNodeTypeList.FindIndex(x => x == roomNodeType);
            var sellection = EditorGUILayout.Popup("", sellected, GetRoomNodeTypeToDisplay());

            roomNodeType = actorTypeList.roomNodeTypeList[sellection];
        }

        if(EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(this);
        
        GUILayout.EndArea();
    }

    private string[] GetRoomNodeTypeToDisplay()
    {
        var localRoomNodeTypeList = actorTypeList.roomNodeTypeList;
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
        roomNodeGraph.SetNodeToDrawConnectionLineFrom(node:this, position: currentEvent.mousePosition );
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
