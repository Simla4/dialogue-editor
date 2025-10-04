using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class NodeGraphTest
{
    [Test]
    public void NodeGraphTestSimplePasses()
    {
        var noneNode = GameResources.Instance.actorTypeList.actorNodeTypeList.Find(x => x.isNone);
        Assert.NotNull(noneNode);

    }
}
