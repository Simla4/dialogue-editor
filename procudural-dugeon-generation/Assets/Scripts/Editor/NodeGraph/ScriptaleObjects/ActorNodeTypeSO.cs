using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Room Node", menuName = "ScriptaleObjects/Dungeon Generation/Room Node Type")]

public class ActorNodeTypeSO : ScriptableObject
{
    public string roomNodeTypeName;

    #region Header
    public bool disableInNodeGraphEditor = true;
    #endregion

    #region Header
    public bool isCorridor;
    #endregion

    #region Header
    public bool isCorridorNS;
    #endregion

    #region Header
    public bool isCorridorEW;
    #endregion

    #region Header
    public bool isEnterenceRoom;
    #endregion

    #region Header
    public bool isBossRoom;
    #endregion

    #region Header
    public bool isNone;
    #endregion

    #region Validation

#if UNITY_EDITOR

    private void OnValidate()
    {
        HelperUtility.ValidateCheckEmptyString(this, nameof(roomNodeTypeName), roomNodeTypeName);
    }

#endif

    #endregion


}
