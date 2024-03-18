using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Room Node", menuName = "ScriptaleObjects/Dungeon Generation/Room Node List")]
public class ActorTypeListSO : ScriptableObject
{
    public List<ActorNodeTypeSO> roomNodeTypeList;
}
