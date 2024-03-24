using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


[CreateAssetMenu(fileName = "Dialogue Node", menuName = "ScriptaleObjects/Dungeon Generation/Actor Node List")]
public class ActorTypeListSO : ScriptableObject
{
    public List<ActorTypeSO> actorNodeTypeList;
}
