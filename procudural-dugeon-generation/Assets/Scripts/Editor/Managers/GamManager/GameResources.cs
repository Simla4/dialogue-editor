using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GameResources : MonoBehaviour
{
    #region Variables

    public ActorTypeListSO actorTypeList;

    #endregion

    private static GameResources instance;

    public static GameResources Instance
    {
        get
        {
            if(instance == null)
            {
                instance = Resources.Load<GameResources>("GameResources");
            }

            return instance;
        }
    }

   /* #region Header Dungeon

    [Space(10)]
    [Header("DUNGEON")]
    #endregion

    #region Tooltip
    [Tooltip("Populate with the dungeon RoomNodeTypeListSO")]
    #endregion*/
}
