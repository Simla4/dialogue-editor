using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.MPE;

public class DialogueNodeGraphEditor : EditorWindow
{
    #region Vairables

    private GUIStyle dialogueNodeStyle;
    private GUIStyle selectedDialogueNodeStyle;

    private static DialogueNodeGraphSO currentDialogueNodeGraph;
    private ActorTypeListSO actorTypeList;
    private DialogueNodeSO currentDialogueNode = null;

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
        SelectedDialogueNodeStyle();
    }

    private void OnDisable()
    {
        Selection.selectionChanged -= InspectorSelectionChanged;
    }

    #endregion

    #region OtherMethods

    [MenuItem("Dialogue Node Graph Editor", menuItem = "Window/Dungeon Editor/Dialogue Node Graph Editor")]
    private static void OpenEditorWindow()
    {
        GetWindow<DialogueNodeGraphEditor>("Dialogue Node Graph Editor");
    }

    // The style of the dialogue node
    private void NodeStyle()
    {
        dialogueNodeStyle = new GUIStyle();

        dialogueNodeStyle.normal.background = EditorGUIUtility.Load("node2") as Texture2D;
        dialogueNodeStyle.normal.textColor = Color.white;
        dialogueNodeStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);
        dialogueNodeStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        
        //Load room node types
        actorTypeList = GameResources.Instance.actorTypeList;
    }

    /// <summary>
    /// The style of the dialogue node we have chosen
    /// </summary>
    private void SelectedDialogueNodeStyle()
    {
        selectedDialogueNodeStyle = new GUIStyle();

        selectedDialogueNodeStyle.normal.background = EditorGUIUtility.Load("node2 on") as Texture2D;
        selectedDialogueNodeStyle.normal.textColor = Color.white;
        selectedDialogueNodeStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);
        selectedDialogueNodeStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        
        //Load room node types
        actorTypeList = GameResources.Instance.actorTypeList;
    }
    
    /// <summary>
    /// Open the room node graph editor window if a room node graph scriptable object asset is double clicked in the inspector
    /// </summary>
    [OnOpenAsset(0)]
    public static bool OnDoubleClickedAsset(int instanceID, int line)
    {
        //for load to room type
        var dialogueNodeGraph = EditorUtility.InstanceIDToObject(instanceID) as DialogueNodeGraphSO;

        if (dialogueNodeGraph != null)
        {
            OpenEditorWindow();
            currentDialogueNodeGraph = dialogueNodeGraph;
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
        
        if (currentDialogueNodeGraph != null)
        {
            DrawDraggedLine();
            
            ProcessEvent(Event.current);
            
            DrawDialogueNodeConnection();

            DrawDialogueNodes();
        }
        
        if(GUI.changed)
            Repaint();
    }

    /// <summary>
    /// Draws the background grid view
    /// </summary>
    /// <param name="gridSize"></param>
    /// <param name="gridOpacity"></param>
    /// <param name="gridColor"></param>
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
    
    /// <summary>
    /// Drawing connections when opening the editor again
    /// </summary>
    private void DrawDialogueNodeConnection()
    {
        var dialogueNodeDictionary = currentDialogueNodeGraph.dialogueNodeDictionary;
        
        foreach (var dialogueNode in currentDialogueNodeGraph.dialogueNodeList)
        {
            var childDialogueNode = dialogueNode.childDialogueList;
            if (childDialogueNode.Count > 0)
            {
                foreach (var ChildDialogueNodeID in childDialogueNode)
                {
                    if (dialogueNodeDictionary.ContainsKey(ChildDialogueNodeID))
                    {
                        DrawConnectionLine(dialogueNode, dialogueNodeDictionary[ChildDialogueNodeID]);
                        GUI.changed = true;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Drawing the connection line between two nodes
    /// </summary>
    /// <param name="parentDialogueNode"></param>
    /// <param name="childDialogueNode"></param>
    public void DrawConnectionLine (DialogueNodeSO parentDialogueNode, DialogueNodeSO childDialogueNode)
    {
        var startPos = parentDialogueNode.rect.center;
        var endPos = childDialogueNode.rect.center;

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
        var node = currentDialogueNodeGraph.dialogueNodeToDrawLineFrom;
        var linePos = currentDialogueNodeGraph.linePos;

        
        if (linePos != Vector2.zero)
        {
            Handles.DrawBezier(node.rect.center, linePos, node.rect.center, 
                linePos, Color.white,null, lineWidth);
        }
    }

    private void ProcessEvent(Event currentEvent)
    {
        graphDrag = Vector2.zero;
        
        if (currentDialogueNode == null || currentDialogueNode.isLeftClikDragging == false)
        {
            currentDialogueNode = IsMouseOverDialogueNode(currentEvent);
        }

        if (currentDialogueNode == null || currentDialogueNodeGraph.dialogueNodeToDrawLineFrom != null)
        {
            ProcesDialogueNodeGraphEvent(currentEvent);
        }
        else
        {
            currentDialogueNode.ProcessEvent(currentEvent);
        }
    }

    /// <summary>
    /// Is there a node where we left the line?
    /// </summary>
    private DialogueNodeSO IsMouseOverDialogueNode(Event currentEvent)
    {
        var currentDialogueNodeGraphList = currentDialogueNodeGraph.dialogueNodeList;
        /* if current mouse position equals whatever node position  */
        for (int i = currentDialogueNodeGraphList.Count - 1; i >= 0; i--)
        {
            if (currentDialogueNodeGraphList[i].rect.Contains(currentEvent.mousePosition))
            {
                return currentDialogueNodeGraphList[i];
            }
        }

        return null;
    }

    private void ProcesDialogueNodeGraphEvent(Event currentEvent)
    {
        switch (currentEvent.type)
        {
            case EventType.MouseDown:
                ProccessMouseDownEvent(currentEvent);
                break;
            case EventType.MouseDrag:
                ProcessMouseDragEvent(currentEvent);
                break;
            case EventType.MouseUp:
                ProceesMouseUpEvent(currentEvent);
                break;
                
            default:
                break;
        }
    }

    private void ProcessMouseDragEvent(Event currentEvent)
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
        if (currentDialogueNodeGraph.linePos != null)
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

        for (int i = 0; i < currentDialogueNodeGraph.dialogueNodeList.Count; i++)
        {
            currentDialogueNodeGraph.dialogueNodeList[i].DragNode(dragDelta);
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
             ClearAllSelecetedDialogueNodes();
        }
    }

    private void ProceesMouseUpEvent(Event currentEvent)
    {
        var dialogueNodeToDrawLineFrom = currentDialogueNodeGraph.dialogueNodeToDrawLineFrom;
        if (currentEvent.button == 1 && dialogueNodeToDrawLineFrom != null)
        {
            var dialogueNodeSo = IsMouseOverDialogueNode(currentEvent);

            if (dialogueNodeSo != null)
            {
                if (dialogueNodeToDrawLineFrom.AddChildDialogueNodeToDialogueNode(dialogueNodeSo.id))
                {
                    dialogueNodeSo.AddParentDialogueNodeToDialogueNode(dialogueNodeToDrawLineFrom.id);
                }
            }

            ClearLineDrag();
        }
    }
    
    /// <summary>
    /// Dragging the connection line
    /// </summary>
    /// <param name="delta"></param>
    private void DragConnecionLine(Vector2 delta)
    {
        currentDialogueNodeGraph.linePos += delta;
    }

    /// <summary>
    /// Displaying the menu and options that open when we right-click
    /// </summary>
    /// <param name="mousePosition"></param>
    private void ShowContextMenu(Vector2 mousePosition)
    {
        GenericMenu menu = new GenericMenu();
        
        menu.AddItem(new GUIContent("Create Dialogue Node"), false, CreateDialogueNode, mousePosition);
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("Select All Dialogue Nodes"), false, SelectAllDialogueNodes);
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("Delete Selected Dialogue Node Links"), false, DeleteSelectedDialogueNodeLinks);
        menu.AddItem(new GUIContent("Delete Selected Dialogue Node"), false, DeleteSelectedDialogueNode);
        
        
        //if you right click, show context menu
        menu.ShowAsContext();
    }

    /// <summary>
    /// Adding a new node to the editor
    /// </summary>
    /// <param name="mousePosOject"></param>
    private void CreateDialogueNode(object mousePosOject)
    {
        CreateDialogueNode(mousePosOject, actorTypeList.actorNodeTypeList.Find(x => x.isNone));
    }

    private void CreateDialogueNode(object mousePosObject, ActorTypeSO actorType)
    {
        var mousePos = (Vector2)mousePosObject;

        var dialogueNodeSo = ScriptableObject.CreateInstance<DialogueNodeSO>();
        
        currentDialogueNodeGraph.dialogueNodeList.Add(dialogueNodeSo);
        dialogueNodeSo.Initialize(new Rect(mousePos, new Vector2(nodeWidht, nodeHeight)), currentDialogueNodeGraph, actorType);
        
        AssetDatabase.AddObjectToAsset(dialogueNodeSo, currentDialogueNodeGraph);
        AssetDatabase.SaveAssets();
        
        currentDialogueNodeGraph.OnValidate();
    }

    /// <summary>
    /// Draw room nodes in the graph window
    /// </summary>
    private void DrawDialogueNodes()
    {
        foreach (var dialogueNodeSo in currentDialogueNodeGraph.dialogueNodeList)
        {
            if (dialogueNodeSo.isSelected)
            {
                dialogueNodeSo.Draw(selectedDialogueNodeStyle);
            }
            else
            {
                dialogueNodeSo.Draw(dialogueNodeStyle);
            }
        }

        GUI.changed = true;
    }

    private void InspectorSelectionChanged()
    {
        var dialogueNodeGraph = Selection.activeObject as DialogueNodeGraphSO;

        if (dialogueNodeGraph != null)
        {
            currentDialogueNodeGraph = dialogueNodeGraph;
            GUI.changed = true;
        }
    }

    private void SelectAllDialogueNodes()
    {
        foreach (var dialogueNodeSo in currentDialogueNodeGraph.dialogueNodeList)
        {
            dialogueNodeSo.isSelected = true;
        }

        GUI.changed = true;
    }

    private void DeleteSelectedDialogueNode()
    {
        Queue<DialogueNodeSO> dialogueNodeDeletionQueue = new Queue<DialogueNodeSO>();
        foreach (var dialogueNodeSo in currentDialogueNodeGraph.dialogueNodeList)
        {
            if (dialogueNodeSo.isSelected && !dialogueNodeSo.actorType.isEnterence)
            {
                dialogueNodeDeletionQueue.Enqueue(dialogueNodeSo);
                foreach (var dialogueNodeID in dialogueNodeSo.childDialogueList)
                {
                    var childDialogueNode = currentDialogueNodeGraph.GetDialogueNode(dialogueNodeID);
                    
                    if (childDialogueNode != null)
                    {
                        childDialogueNode.RemoveParentDialogueNodeIDFromNode(dialogueNodeSo.id);
                    }
                }

                foreach (var parentDialogueNodeID in dialogueNodeSo.parentDialogueList)
                {
                    var parentDialogueNode = currentDialogueNodeGraph.GetDialogueNode(parentDialogueNodeID);
                    
                    if (parentDialogueNodeID != null)
                    {
                        parentDialogueNode.RemoveChildDialogueNodeIDFromNode(dialogueNodeSo.id);
                    }
                }
            }
        }

        while (dialogueNodeDeletionQueue.Count > 0)
        {
            var dialogueNodeToDelete = dialogueNodeDeletionQueue.Dequeue();

            currentDialogueNodeGraph.dialogueNodeDictionary.Remove(dialogueNodeToDelete.id);
            currentDialogueNodeGraph.dialogueNodeList.Remove(dialogueNodeToDelete);
            
            //Remove from the asset database
            DestroyImmediate(dialogueNodeToDelete, true);
            //Save asset data base
            AssetDatabase.SaveAssets();
        }
    }

    private void DeleteSelectedDialogueNodeLinks()
    {
        foreach (var dialogueNode in currentDialogueNodeGraph.dialogueNodeList)
        {
            var childDialogueList = dialogueNode.childDialogueList;
            if (dialogueNode.isSelected && childDialogueList.Count > 0)
            {
                for (int i = childDialogueList.Count - 1; i >= 0; i--)
                {
                    var childDialogueNode = currentDialogueNodeGraph.GetDialogueNode(childDialogueList[i]);

                    if (childDialogueNode != null && childDialogueNode.isSelected)
                    {
                        dialogueNode.RemoveChildDialogueNodeIDFromNode(childDialogueNode.id);
                        childDialogueNode.RemoveParentDialogueNodeIDFromNode(dialogueNode.id);
                    }
                }
            }
        }
        
        ClearAllSelecetedDialogueNodes();
    }

    private void ClearAllSelecetedDialogueNodes()
    {
        foreach (var dialogueNode in currentDialogueNodeGraph.dialogueNodeList)
        {
            if (dialogueNode.isSelected)
            {
                dialogueNode.isSelected = false;
                GUI.changed = true;
            }
        }
    }
    
    /// <summary>
    /// İf there is no connection, clear the line
    /// </summary>
    private void ClearLineDrag()
    {
        currentDialogueNodeGraph.dialogueNodeToDrawLineFrom = null;
        currentDialogueNodeGraph.linePos = Vector2.zero;
        GUI.changed = true;
    }

    #endregion
}
