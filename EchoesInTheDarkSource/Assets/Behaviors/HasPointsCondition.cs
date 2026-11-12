using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;

[Serializable]
[Condition(name: "HasPoints", story: "Has items in [Waypoints]", category: "Conditions", id: "2a5360e2d6675337c79c0003f92a0cd3")]
public class HasPointsCondition : Condition
{
    [SerializeReference] public BlackboardVariable<List<GameObject>> Waypoints;

    public override bool IsTrue()
    {
        return Waypoints.Value != null && Waypoints.Value.Count > 0;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
