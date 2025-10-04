using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Dialogue Node", menuName = "ScriptaleObjects/Dungeon Generation/Actor Type")]

public class ActorTypeSO : ScriptableObject
{
    public string actorName;
    public string actorDescription;
    public Sprite actorIcon;

    #region Header
    public bool disableInNodeGraphEditor = true;
    #endregion

    #region Header
    public bool isEnterence;
    #endregion

    #region Header
    public bool isNone;
    #endregion

    #region Validation

#if UNITY_EDITOR

    private void OnValidate()
    {
        HelperUtility.ValidateCheckEmptyString(this, nameof(actorName), actorName);
    }

#endif

    #endregion


}
