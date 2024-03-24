using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Dialogue Node", menuName = "ScriptaleObjects/Dungeon Generation/Room Node List")]
public class ActorTypeListSO : ScriptableObject
{
    public List<ActorNodeTypeSO> roomNodeTypeList;
}
