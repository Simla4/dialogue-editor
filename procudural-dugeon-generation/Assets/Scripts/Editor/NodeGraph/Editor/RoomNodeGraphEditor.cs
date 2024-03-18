using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.MPE;

public class RoomNodeGraphEditor : EditorWindow
{
    #region Vairables

    private GUIStyle roomNodeStyle;
    private GUIStyle selectedRoomNodeStyle;

    private static ActorNodeGraphSO currentActorNodeGraph;
    private ActorTypeListSO actorTypeList;
    private ActorNodeSO currentActorNode = null;

    private Vector2 graphOffset;
    private Vector2 graphDrag;

    private const float nodeWidht = 160f;
    private const float nodeHeight = 75;
    private const int nodeBorder = 12;
    private const int nodePadding = 25;
    
    private const float lineWidth = 5f;
    private const float connectingLineArrowSize = 6f;

    private const float largeGridSize = 100f;
    private const float smallGridSize = 25f;

    #endregion

    #region CallBacks

    private void OnEnable()
    {
        Selection.selectionChanged += InspectorSelectionChanged;
        
        NodeStyle();
        SelectedRoomNodeStyle();
    }

    private void OnDisable()
    {
        Selection.selectionChanged -= InspectorSelectionChanged;
    }

    #endregion

    #region OtherMethods

    [MenuItem("Room Node Graph Editor", menuItem = "Window/Dungeon Editor/Room Node Graph Editor")]
    private static void OpenEditorWindow()
    {
        GetWindow<RoomNodeGraphEditor>("Romm Node Graph Editor");
    }

    private void NodeStyle()
    {
        roomNodeStyle = new GUIStyle();

        roomNodeStyle.normal.background = EditorGUIUtility.Load("node2") as Texture2D;
        roomNodeStyle.normal.textColor = Color.white;
        roomNodeStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);
        roomNodeStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        
        //Load room node types
        actorTypeList = GameResources.Instance.actorTypeList;
    }

    private void SelectedRoomNodeStyle()
    {
        selectedRoomNodeStyle = new GUIStyle();

        selectedRoomNodeStyle.normal.background = EditorGUIUtility.Load("node2 on") as Texture2D;
        selectedRoomNodeStyle.normal.textColor = Color.white;
        selectedRoomNodeStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);
        selectedRoomNodeStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        
        //Load room node types
        actorTypeList = GameResources.Instance.actorTypeList;
    }
    
    /// <summary>
    /// Open the room node graph editor window if a room node graph scriptable object asset is double clicked in the inspectoe
    /// </summary>
    [OnOpenAsset(0)]
    public static bool OnDoubleClickedAsset(int instanceID, int line)
    {
        //for load to room type
        var roomNodeGraph = EditorUtility.InstanceIDToObject(instanceID) as ActorNodeGraphSO;

        if (roomNodeGraph != null)
        {
            OpenEditorWindow();
            currentActorNodeGraph = roomNodeGraph;
            return true;
        }

        return false;
    }
    
        
    /// <summary>
    /// Draw Editor GUI
    /// </summary>
    private void OnGUI()
    {
        DrawBackgroundGrid(largeGridSize, 1, Color.gray);
        DrawBackgroundGrid(smallGridSize, 0.5f, Color.gray);
        
        if (currentActorNodeGraph != null)
        {
            DrawDraggedLine();
            
            ProcessEvent(Event.current);
            
            DrawRoomNodeConnection();

            DrawRoomNodes();
        }
        
        if(GUI.changed)
            Repaint();
    }

    private void DrawBackgroundGrid(float gridSize, float gridOpacity, Color gridColor)
    {
        var verticalLineCount = Mathf.CeilToInt((position.width + gridSize) / gridSize);
        var horizontalLineCount = Mathf.CeilToInt((position.height + gridSize) / gridSize);

        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

        graphOffset += graphDrag * 0.5f;

        var gridOffet = new Vector3(graphOffset.x % gridSize, graphOffset.y % gridSize, 0);

        for (int i = 0; i < verticalLineCount; i++)
        {
            Handles.DrawLine(new Vector3(gridSize * i, -gridSize, 0) + gridOffet, new Vector3(gridSize * i, position.height + gridSize, 0) + gridOffet);   
        }

        for (int j = 0; j < horizontalLineCount; j++)
        {
            Handles.DrawLine(new Vector3( -gridSize, gridSize * j, 0) + gridOffet, new Vector3(position.width + gridSize, gridSize * j, 0) + gridOffet);
        }
        
        Handles.color = Color.white;
    }

    private void DrawRoomNodeConnection()
    {
        var roomNodeDictionary = currentActorNodeGraph.roomNodeDictionary;
        
        foreach (var roomNode in currentActorNodeGraph.roomNodeList)
        {
            var childRoomNode = roomNode.childRoomList;
            if (childRoomNode.Count > 0)
            {
                foreach (var ChildRoomNodeID in childRoomNode)
                {
                    if (roomNodeDictionary.ContainsKey(ChildRoomNodeID))
                    {
                        DrawConnectionLine(roomNode, roomNodeDictionary[ChildRoomNodeID]);
                        GUI.changed = true;
                    }
                }
            }
        }
    }

    public void DrawConnectionLine (ActorNodeSO parentActorNode, ActorNodeSO childActorNode)
    {
        var startPos = parentActorNode.rect.center;
        var endPos = childActorNode.rect.center;

        // calculate direction
        var direction = endPos - startPos;
        
        //calculate vector center
        var center = (endPos + startPos) / 2f;

        // calculate nolmalize perpendicular position from the center
        var arrowTailPoint1 = center - new Vector2(-direction.y, direction.x).normalized * connectingLineArrowSize;
        var arrowTailPoint2 = center + new Vector2(-direction.y, direction.x).normalized * connectingLineArrowSize;
        
        //calculate center offset position for arrow head
        var arrowHeadPoint = center + direction.normalized * connectingLineArrowSize;
        
        //Draw Arrow
        Handles.DrawBezier(arrowHeadPoint, arrowTailPoint1, arrowHeadPoint, arrowTailPoint1, Color.white, null, lineWidth);
        Handles.DrawBezier(arrowHeadPoint, arrowTailPoint2, arrowHeadPoint,arrowTailPoint2, Color.white, null, lineWidth);
        
        //Draw line
        Handles.DrawBezier(startPos, endPos, startPos, endPos, Color.white, null, lineWidth);

        GUI.changed = true;
    }

    private void DrawDraggedLine()
    {
        var node = currentActorNodeGraph.actorNodeToDrawLineFrom;
        var linePos = currentActorNodeGraph.linePos;

        
        if (linePos != Vector2.zero)
        {
            Handles.DrawBezier(node.rect.center, linePos, node.rect.center, 
                linePos, Color.white,null, lineWidth);
        }
    }

    private void ProcessEvent(Event currentEvent)
    {
        graphDrag = Vector2.zero;
        
        if (currentActorNode == null || currentActorNode.isLeftClikDragging == false)
        {
            currentActorNode = IsMouseOverRoomNode(currentEvent);
        }

        if (currentActorNode == null || currentActorNodeGraph.actorNodeToDrawLineFrom != null)
        {
            ProcesRoomNodeGraphEvent(currentEvent);
        }
        else
        {
            currentActorNode.ProcessEvent(currentEvent);
        }
    }

    private ActorNodeSO IsMouseOverRoomNode(Event currentEvent)
    {
        var currentRoomNodeGraphList = currentActorNodeGraph.roomNodeList;
        /* if current mouse position equals whatever node position  */
        for (int i = currentRoomNodeGraphList.Count - 1; i >= 0; i--)
        {
            if (currentRoomNodeGraphList[i].rect.Contains(currentEvent.mousePosition))
            {
                return currentRoomNodeGraphList[i];
            }
        }

        return null;
    }

    private void ProcesRoomNodeGraphEvent(Event currentEvent)
    {
        switch (currentEvent.type)
        {
            case EventType.MouseDown:
                ProccessMouseDownEvent(currentEvent);
                break;
            case EventType.MouseDrag:
                ProccessMouseDragEvent(currentEvent);
                break;
            case EventType.MouseUp:
                ProceesMouseUpEvent(currentEvent);
                break;
                
            default:
                break;
        }
    }

    private void ProccessMouseDragEvent(Event currentEvent)
    {
        if (currentEvent.button == 1)
        {
            ProcessRightClikDragEvent(currentEvent);
        }

        if (currentEvent.button == 0)
        {
            ProcessLeftClickDragEvent(currentEvent.delta);
        }
    }

    private void ProcessRightClikDragEvent(Event currentEvent)
    {
        if (currentActorNodeGraph.linePos != null)
        {
            DragConnecionLine(currentEvent.delta);
            GUI.changed = true;
        }
    }
    /// <summary>
    /// Process Left click event - drag room node graph
    /// </summary>
    /// <param name="dragDelta"></param>
    private void ProcessLeftClickDragEvent(Vector2 dragDelta)
    {
        graphDrag = dragDelta;

        for (int i = 0; i < currentActorNodeGraph.roomNodeList.Count; i++)
        {
            currentActorNodeGraph.roomNodeList[i].DragNode(dragDelta);
        }

        GUI.changed = true;
    }


    /// <summary>
    /// when right clicked mose down, show the context menu
    /// </summary>
    private void ProccessMouseDownEvent(Event currentEvent)
    {
        // did you right click?
        if (currentEvent.button == 1)
        {
            ShowContextMenu(currentEvent.mousePosition);
        }
        else if(currentEvent.button == 0)
        {
             ClearLineDrag();
             ClearAllSelecetedRoomNodes();
        }
    }

    private void ProceesMouseUpEvent(Event currentEvent)
    {
        var roomNodeToDrawLineFrom = currentActorNodeGraph.actorNodeToDrawLineFrom;
        if (currentEvent.button == 1 && roomNodeToDrawLineFrom != null)
        {
            var roomNode = IsMouseOverRoomNode(currentEvent);

            if (roomNode != null)
            {
                if (roomNodeToDrawLineFrom.AddChildRoomNodeToRoomNoode(roomNode.id))
                {
                    roomNode.AddParentRoomNodeToRoomNode(roomNodeToDrawLineFrom.id);
                }
            }

            ClearLineDrag();
        }
    }
    
    
    private void DragConnecionLine(Vector2 delta)
    {
        currentActorNodeGraph.linePos += delta;
    }

    private void ShowContextMenu(Vector2 mousePosition)
    {
        GenericMenu menu = new GenericMenu();
        
        menu.AddItem(new GUIContent("Create Room Node"), false, CreateRoomNode, mousePosition);
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("Select All Room Nodes"), false, SelectAllRoomNodes);
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("Delete Selected Room Node Links"), false, DeleteSelectedRoomNodeLinks);
        menu.AddItem(new GUIContent("Delete Selected Room Node"), false, DeleteSelectedRoomNode);
        
        
        //if you right click, show context menu
        menu.ShowAsContext();
    }

    private void CreateRoomNode(object mousePosOject)
    {
        CreateRoomNode(mousePosOject, actorTypeList.roomNodeTypeList.Find(x => x.isNone));
    }

    private void CreateRoomNode(object mousePosObject, ActorNodeTypeSO actorNodeType)
    {
        var mousePos = (Vector2)mousePosObject;

        var roomNode = ScriptableObject.CreateInstance<ActorNodeSO>();
        
        currentActorNodeGraph.roomNodeList.Add(roomNode);
        roomNode.Initialize(new Rect(mousePos, new Vector2(nodeWidht, nodeHeight)), currentActorNodeGraph, actorNodeType);
        
        AssetDatabase.AddObjectToAsset(roomNode, currentActorNodeGraph);
        AssetDatabase.SaveAssets();
        
        currentActorNodeGraph.OnValidate();
    }

    /// <summary>
    /// Draw room nodes in the graph window
    /// </summary>
    private void DrawRoomNodes()
    {
        foreach (var roomNode in currentActorNodeGraph.roomNodeList)
        {
            if (roomNode.isSelected)
            {
                roomNode.Draw(selectedRoomNodeStyle);
            }
            else
            {
                roomNode.Draw(roomNodeStyle);
            }
        }

        GUI.changed = true;
    }

    private void InspectorSelectionChanged()
    {
        var roomNodeGraph = Selection.activeObject as ActorNodeGraphSO;

        if (roomNodeGraph != null)
        {
            currentActorNodeGraph = roomNodeGraph;
            GUI.changed = true;
        }
    }

    private void SelectAllRoomNodes()
    {
        foreach (var roomNode in currentActorNodeGraph.roomNodeList)
        {
            roomNode.isSelected = true;
        }

        GUI.changed = true;
    }

    private void DeleteSelectedRoomNode()
    {
        Queue<ActorNodeSO> roomNodeDeletionQueue = new Queue<ActorNodeSO>();
        foreach (var roomNode in currentActorNodeGraph.roomNodeList)
        {
            if (roomNode.isSelected && !roomNode.actorNodeType.isEnterenceRoom)
            {
                roomNodeDeletionQueue.Enqueue(roomNode);
                foreach (var childRoomNodeID in roomNode.childRoomList)
                {
                    var childRoomNode = currentActorNodeGraph.GetRoomNode(childRoomNodeID);
                    
                    if (childRoomNode != null)
                    {
                        childRoomNode.RemoveParentRoomNodeIDFromNode(roomNode.id);
                    }
                }

                foreach (var parentRoomNodeID in roomNode.parentRoomList)
                {
                    var parentRoomNode = currentActorNodeGraph.GetRoomNode(parentRoomNodeID);
                    
                    if (parentRoomNodeID != null)
                    {
                        parentRoomNode.RemoveChildRoomNodeIDFromNode(roomNode.id);
                    }
                }
            }
        }

        while (roomNodeDeletionQueue.Count > 0)
        {
            var roomNodeToDelete = roomNodeDeletionQueue.Dequeue();

            currentActorNodeGraph.roomNodeDictionary.Remove(roomNodeToDelete.id);
            currentActorNodeGraph.roomNodeList.Remove(roomNodeToDelete);
            
            //Remove from the asset database
            DestroyImmediate(roomNodeToDelete, true);
            //Save asset data base
            AssetDatabase.SaveAssets();
        }
    }

    private void DeleteSelectedRoomNodeLinks()
    {
        foreach (var roomNode in currentActorNodeGraph.roomNodeList)
        {
            var childRoomList = roomNode.childRoomList;
            if (roomNode.isSelected && childRoomList.Count > 0)
            {
                for (int i = childRoomList.Count - 1; i >= 0; i--)
                {
                    var childRoomNode = currentActorNodeGraph.GetRoomNode(childRoomList[i]);

                    if (childRoomNode != null && childRoomNode.isSelected)
                    {
                        roomNode.RemoveChildRoomNodeIDFromNode(childRoomNode.id);
                        childRoomNode.RemoveParentRoomNodeIDFromNode(roomNode.id);
                    }
                }
            }
        }
        
        ClearAllSelecetedRoomNodes();
    }

    private void ClearAllSelecetedRoomNodes()
    {
        foreach (var roomNode in currentActorNodeGraph.roomNodeList)
        {
            if (roomNode.isSelected)
            {
                roomNode.isSelected = false;
                GUI.changed = true;
            }
        }
    }
    
    /// <summary>
    /// İf there is no connection, claear the line
    /// </summary>
    private void ClearLineDrag()
    {
        currentActorNodeGraph.actorNodeToDrawLineFrom = null;
        currentActorNodeGraph.linePos = Vector2.zero;
        GUI.changed = true;
    }

    #endregion
}
